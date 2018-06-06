namespace D365.SQL.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Common;
    using Configuration;
    using D365;
    using DML.Select;
    using DML.Select.Columns;
    using DML.Select.From;
    using DML.Select.Order;
    using DML.Select.Where;
    using Microsoft.Xrm.Sdk.Query;

    internal class SelectEngine
    {
        public SelectEngine(CRMInstance crmInstance, ISqlEngineConfiguration configuration)
        {
            CRMInstance = crmInstance;
            Configuration = configuration;
        }

        private DataTable ExecuteInformationSchema(SelectStatement statement)
        {
            var metadata = new MetadataManager(CRMInstance);

            var entityName = ((SelectFrom)statement.From.First()).Name;

            switch (entityName)
            {
                case "information_schema.tables":

                    break;

                default:
                    throw new Exception($"Information Schema '{entityName}' unsupported.");
            }

            return null;
        }

        private DataTable ExecuteExpression(SelectStatement statement)
        {
            var metadata = new MetadataManager(CRMInstance);

            var entityName = ((SelectFrom)statement.From.First()).Name;

            var query = new QueryExpression(entityName);

            if (statement.Columns.Any())
            {
                query.ColumnSet = new ColumnSet(false);

                // Until caching layer is ready we will retrieve fields every time
                var entityFields = metadata.GetEntityFields(entityName);

                if (entityFields == null)
                {
                    throw new Exception($"Unable to read entity '{entityName}' fields.");
                }
                
                if (statement.Columns.Any(x => x.Column.Type.In(SelectColumnTypeEnum.All, SelectColumnTypeEnum.System)))
                {
                    for (int i = 0; i < statement.Columns.Count; i++)
                    {
                        var selectColumn = statement.Columns[i];
                        var column = selectColumn.Column;

                        if (column.Type == SelectColumnTypeEnum.All)
                        {
                            foreach (var entityField in entityFields)
                            {
                                var fieldSelectColumn = new FieldSelectColumn(entityField.SchemaName);

                                statement.Columns.Insert(i++, new SelectColumn(fieldSelectColumn, null));
                            }
                        }
                        else if (column.Type == SelectColumnTypeEnum.System)
                        {
                            foreach (var fieldName in Configuration.Settings.SystemSelectFields)
                            {
                                if (fieldName.IsEmpty())
                                {
                                    throw new Exception($"'{fieldName}' not set.");
                                }

                                if (string.Equals(fieldName, "$id", StringComparison.OrdinalIgnoreCase))
                                {
                                    var idEntityField = entityFields.Single(x => x.IsPrimaryId);

                                    var fieldSelectColumn = new FieldSelectColumn(idEntityField.SchemaName);

                                    statement.Columns.Insert(i++, new SelectColumn(fieldSelectColumn, null));
                                }
                                else if (string.Equals(fieldName, "$name", StringComparison.OrdinalIgnoreCase))
                                {
                                    var nameEntityField = entityFields.Single(x => x.IsPrimaryName);

                                    var fieldSelectColumn = new FieldSelectColumn(nameEntityField.SchemaName);

                                    statement.Columns.Insert(i++, new SelectColumn(fieldSelectColumn, null)); 
                                }
                                else if (fieldName.StartsWith("$"))
                                {
                                    throw new Exception($"System field '{fieldName}' not supported.");
                                }
                                else
                                {
                                    var fieldSelectColumn = new FieldSelectColumn(fieldName);

                                    statement.Columns.Insert(i++, new SelectColumn(fieldSelectColumn, null));
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < statement.Columns.Count; i++)
                {
                    var selectColumn = statement.Columns[i];
                    var column = selectColumn.Column;

                    if (column.Type == SelectColumnTypeEnum.Field)
                    {
                        var fieldColumn = (FieldSelectColumn)column;
                        query.ColumnSet.AddColumn(fieldColumn.Name);
                    }
                    else if (column.Type == SelectColumnTypeEnum.InnerSelect)
                    {
                        throw new Exception("Inner selects are currently unsupported.");
                    }
                    else if (column.Type == SelectColumnTypeEnum.Function)
                    {
                        throw new Exception("Columns with functions are currently unsupported.");
                    }
                }
            }
            else
            {
                query.ColumnSet = new ColumnSet(true);
            }

            if (statement.Top.HasValue)
            {
                query.TopCount = statement.Top.Value;
            }

            if (statement.Where.Any())
            {
                for (var i = 0; i < statement.Where.Count; i++)
                {
                    var whereClause = statement.Where[i];

                    if (whereClause.Type == SelectWhereTypeEnum.Comparison)
                    {
                        var comparisonClause = (SelectWhereComparison)whereClause;

                        var conditionName = GetWhereClauseAttributeName(comparisonClause.LeftExpression);
                        var conditionValue = GetWhereClauseAttributeName(comparisonClause.RightExpression);

                        var conditionOperator = ConditionOperator.Equal;

                        switch (comparisonClause.Operator)
                        {
                            case SelectWhereComparisonOperatorEnum.Equal:
                                conditionOperator = comparisonClause.Negate
                                    ? ConditionOperator.NotEqual
                                    : ConditionOperator.Equal;
                                break;

                            case SelectWhereComparisonOperatorEnum.NotEqual:
                                conditionOperator = comparisonClause.Negate
                                    ? ConditionOperator.Equal
                                    : ConditionOperator.NotEqual;
                                break;

                            case SelectWhereComparisonOperatorEnum.LessThan:
                                conditionOperator = comparisonClause.Negate
                                    ? ConditionOperator.GreaterEqual
                                    : ConditionOperator.LessThan;
                                break;

                            case SelectWhereComparisonOperatorEnum.LessEqualThan:
                                conditionOperator = comparisonClause.Negate
                                    ? ConditionOperator.GreaterThan
                                    : ConditionOperator.LessEqual;
                                break;

                            case SelectWhereComparisonOperatorEnum.GreaterThan:
                                conditionOperator = comparisonClause.Negate
                                    ? ConditionOperator.LessEqual
                                    : ConditionOperator.GreaterThan;
                                break;

                            case SelectWhereComparisonOperatorEnum.GreaterEqualThan:
                                conditionOperator = comparisonClause.Negate
                                    ? ConditionOperator.LessThan
                                    : ConditionOperator.GreaterEqual;
                                break;

                            default:
                                throw new Exception($"Found unexpected comparison operator '{comparisonClause.Operator}'");
                        }

                        query.Criteria.AddCondition(conditionName, conditionOperator, conditionValue);
                    }
                    else if (whereClause.Type == SelectWhereTypeEnum.Null)
                    {
                        var nullClause = (SelectWhereNull)whereClause;

                        var conditionName = GetWhereClauseAttributeName(nullClause.Expression);

                        query.Criteria.AddCondition(conditionName, nullClause.Negate ? ConditionOperator.NotNull : ConditionOperator.Null);
                    }
                    else if (whereClause.Type == SelectWhereTypeEnum.Like)
                    {
                        var likeClause = (SelectWhereLike)whereClause;

                        var conditionName = GetWhereClauseAttributeName(likeClause.LeftExpression);
                        var conditionValue = GetWhereClauseAttributeName(likeClause.RightExpression);

                        query.Criteria.AddCondition(conditionName, likeClause.Negate ? ConditionOperator.NotLike : ConditionOperator.Like, conditionValue);
                    }
                    else if (whereClause.Type != SelectWhereTypeEnum.Operator)
                    {
                        throw new Exception($"Found unexpected where clause {whereClause.Type}");
                    }
                }
            }

            if (statement.Order.Any())
            {
                foreach (var orderItem in statement.Order)
                {
                    if (orderItem.Type == SelectOrderTypeEnum.Column)
                    {
                        var columnOrder = (SelectOrderColumn)orderItem;
                        var direction = orderItem.Direction == OrderDirection.Asc ? OrderType.Ascending : OrderType.Descending;

                        query.AddOrder(columnOrder.Name, direction);
                    }
                    else if (orderItem.Type == SelectOrderTypeEnum.Position)
                    {
                        var columnPosition = (SelectOrderPosition)orderItem;
                        var direction = orderItem.Direction == OrderDirection.Asc ? OrderType.Ascending : OrderType.Descending;

                        var fieldColumn = statement.Columns[columnPosition.Position - 1].Column as FieldSelectColumn;

                        if (fieldColumn == null)
                        {
                            throw new Exception($"Position '{columnPosition.Position}' must point to a CRM field");
                        }

                        query.AddOrder(fieldColumn.Name, direction);
                    }
                    else
                    {
                        throw new Exception($"Unsupported order type '{orderItem.Type}'");
                    }
                }
            }

            var entities = CRMInstance.Service.RetrieveMultiple(query).Entities.ToList();

            var list = new List<List<KeyValuePair<string, object>>>();

            foreach (var entity in entities)
            {
                var values = new List<KeyValuePair<string, object>>();

                foreach (var attribute in entity.Attributes)
                {
                    object value = null;
                    var type = attribute.Value.GetType();

                    if (type != typeof(string))
                    {
                        if (entity.FormattedValues.ContainsKey(attribute.Key))
                        {
                            value = entity.FormattedValues[attribute.Key];
                        }
                    }

                    if (value == null)
                    {
                        value = Convert.ToString(attribute.Value);
                    }

                    values.Add(new KeyValuePair<string, object>(attribute.Key, value));
                }

                list.Add(values);
            }

            return list.ConvertToDataTable(statement.Columns);
        }

        public DataTable Execute(SelectStatement statement)
        {
            ValidateStatement(statement);

            var entityName = ((SelectFrom)statement.From.First()).Name;

            if (entityName.StartsWith("information_schema."))
            {
                return ExecuteInformationSchema(statement);
            }

            return ExecuteExpression(statement);
        }

        private string GetWhereClauseAttributeName(SelectWhereColumnBase whereColumn)
        {
            switch (whereColumn.Type)
            {
                case SelectColumnTypeEnum.Field:
                    var fieldColumn = (SelectWhereFieldColumn) whereColumn;

                    return fieldColumn.Name;

                case SelectColumnTypeEnum.Raw:
                    var rawColumn = (SelectWhereRawColumn)whereColumn;

                    return rawColumn.Value.CleanRaw();
            }

            throw new NotSupportedException();
        }

        private CRMInstance CRMInstance { get; }

        private ISqlEngineConfiguration Configuration { get; }

        private void ValidateStatement(SelectStatement statement)
        {
            if (statement.From.Count > 1)
            {
                throw new Exception("Multiple 'FROM' are currently unsupported.");
            }

            if (statement.From.First() is SelectFrom == false)
            {
                throw new Exception("Only single entity 'FROM' is supported.");
            }
        }
    }
}
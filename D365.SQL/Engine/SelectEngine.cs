namespace D365.SQL.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Common;
    using DML.Select;
    using DML.Select.Columns;
    using DML.Select.From;
    using DML.Select.Order;
    using DML.Select.Where;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Query;

    internal class SelectEngine
    {
        public SelectEngine(OrganizationServiceProxy service)
        {
            Service = service;
        }

        private OrganizationServiceProxy Service { get; set; }

        public DataTable Execute(SelectStatement statement)
        {
            var query = new QueryExpression(((SelectFrom)statement.From.First()).Name);

            if (statement.Columns.Any())
            {
                query.ColumnSet = new ColumnSet(false);

                foreach (var column in statement.Columns)
                {
                    if (column.Type == SelectColumnTypeEnum.Field)
                    {
                        var fieldColumn = (FieldSelectColumn) column;
                        query.ColumnSet.AddColumn(fieldColumn.Name);
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
                        var comparisonClause = (SelectWhereComparison) whereClause;

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
                }
            }

            var entities = Service.RetrieveMultiple(query).Entities.ToList();

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

            return ConvertToDataTable(list, statement.Columns);
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

        public DataTable ConvertToDataTable(List<List<KeyValuePair<string, object>>> crmDataItems, List<SelectColumnBase> columns)
        {
            var dt = new DataTable();

            foreach (var column in columns)
            {
                if (column.Type.In(SelectColumnTypeEnum.All, SelectColumnTypeEnum.System))
                {
                    
                }
                else
                {
                    var name = column.Label;

                    if (column.Type == SelectColumnTypeEnum.Field)
                    {
                        var fieldColumn = (FieldSelectColumn)column;

                        name = fieldColumn.Name;
                    }

                    var dataColumn = dt.Columns.Add(name, column.ValueType);

                    dataColumn.Caption = column.Label;
                }
            }

            var rawColumns = columns.Where(x => x.Type == SelectColumnTypeEnum.Raw).Cast<RawSelectColumn>();

            foreach (var crmDataItem in crmDataItems)
            {
                var row = dt.NewRow();

                foreach (var crmPair in crmDataItem)
                {
                    if (dt.Columns.Contains(crmPair.Key))
                    {
                        row[crmPair.Key] = crmPair.Value;
                    }
                }

                if (rawColumns.Any())
                {
                    foreach (var rawColumn in rawColumns)
                    {
                        var value = rawColumn.Value;

                        if (rawColumn.Value != null && rawColumn.Value.StartsWith("'") && rawColumn.Value.EndsWith("'"))
                        {
                            value = rawColumn.Value.Substring(1, rawColumn.Value.Length - 2);
                        }

                        row[rawColumn.Label] = value;
                    }
                }

                dt.Rows.Add(row);
            }

            // caption is broken in linqpad so we are always using  label as column name
            foreach (DataColumn dataColumn in dt.Columns)
            {
                if (dataColumn.Caption.StartsWith("'") && dataColumn.Caption.EndsWith("'"))
                {
                    dataColumn.Caption = dataColumn.Caption.Substring(1, dataColumn.Caption.Length - 2);
                }

                dataColumn.ColumnName = dataColumn.Caption;
            }

            return dt;
        }
    }
}
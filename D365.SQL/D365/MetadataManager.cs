namespace D365.SQL.D365
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Common;
    using Engine.Storage.InMemory;
    using log4net;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;

    internal class MetadataManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MetadataManager));

        public MetadataManager(CRMInstance crmInstance)
        {
            CRMInstance = crmInstance;
        }

        private CRMInstance CRMInstance { get; }

        public List<CRMEntityField> GetEntityFields(string entityName)
        {
            try
            {
                var metaDataRequest = new RetrieveEntityRequest
                {
                    EntityFilters = EntityFilters.Attributes,
                    LogicalName = entityName,
                };

                var retrieveEntityResponse = (RetrieveEntityResponse)CRMInstance.Service.Execute(metaDataRequest);

                var attributes = retrieveEntityResponse.EntityMetadata.Attributes.ToList();

                var entityFields = new List<CRMEntityField>();

                foreach (var attribute in attributes.OrderBy(x => x.LogicalName))
                {
                    if (attribute.IsValidForRead.HasValue == false || attribute.IsValidForRead.Value == false)
                    {
                        continue;
                    }

                    if (attribute.AttributeOf.IsNotEmpty())
                    {
                        continue;
                    }

                    var entityField = new CRMEntityField()
                    {
                        SchemaName = attribute.LogicalName,
                        IsPrimaryId = string.Equals(attribute.LogicalName, $"{entityName}id", StringComparison.OrdinalIgnoreCase),
                        IsPrimaryName = attribute.IsPrimaryName.HasValue && attribute.IsPrimaryName.Value
                                        &&
                                        string.Equals(attribute.EntityLogicalName, entityName, StringComparison.OrdinalIgnoreCase)
                    };

                    entityFields.Add(entityField);
                }

                return entityFields;
            }
            catch (Exception ex)
            {
                Log.Warn($"Unable to read entity fields for entity '{entityName}'", ex);
            }

            return null;
        }

        public InMemoryTable GetEntities()
        {
            var retrieveAllEntitiesRequest = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity
            };

            var retrieveAllEntitiesResponse = (RetrieveAllEntitiesResponse)CRMInstance.Service.Execute(retrieveAllEntitiesRequest);

            var props = typeof(EntityMetadata).GetProperties();
            
            var propsItems = new List<PropItem>();
            var columns = new List<ColumnInfo>();

            Func<Type, bool> isSupported = (type) =>
            {
                if (type == null)
                {
                    return false;
                }

                if (type.IsPrimitive 
                    || type == typeof(string) 
                    || type == typeof(BooleanManagedProperty) 
                    || type == typeof(Guid)
                    || type == typeof(Label)
                    || type.IsEnum)
                {
                    return true;
                }

                return false;
            };

            foreach (var prop in props.OrderBy(x => x.Name))
            {
                var isNullable = prop.PropertyType.IsNullable();
                
                var propType = isNullable ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType;

                if (propType == null)
                {
                    continue;
                }

                if (isSupported(propType) == false)
                {
                    continue;
                }

                propsItems.Add(new PropItem() { PropertyInfo = prop, Type = propType });

                if (propType == typeof(BooleanManagedProperty))
                {
                    columns.Add(new ColumnInfo(prop.Name, typeof(bool)));
                }
                else if (propType == typeof(Label))
                {
                    columns.Add(new ColumnInfo(prop.Name, typeof(string)));
                }
                else if (propType.IsEnum)
                {
                    columns.Add(new ColumnInfo($"{prop.Name}Id", typeof(int)));
                    columns.Add(new ColumnInfo(prop.Name, typeof(string)));
                }
                else
                {
                    columns.Add(new ColumnInfo(prop.Name, propType));
                }
            }

            columns.MoveItemsToStart(
                (source, destination) => string.Equals(source.Name, destination, StringComparison.OrdinalIgnoreCase),
                "MetadataId", "SchemaName", "LogicalName", "DisplayName", "Description", "PrimaryIdAttribute", "PrimaryNameAttribute");

            var table = new InMemoryTable(null, columns);

            foreach (var entityMetadata in retrieveAllEntitiesResponse.EntityMetadata)
            {
                var row = new InMemoryTableRow();

                foreach (var propsItem in propsItems)
                {
                    if (isSupported(propsItem.Type) == false)
                    {
                        continue;
                    }

                    if (propsItem.Type == typeof(BooleanManagedProperty))
                    {
                        var value = propsItem.PropertyInfo.GetValue(entityMetadata);

                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        else
                        {
                            var boolManaged = (BooleanManagedProperty) value;

                            value = boolManaged.Value;
                        }

                        row.Columns.Add(new InMemoryColumn(propsItem.PropertyInfo.Name, value));
                    }
                    else if (propsItem.Type == typeof(Label))
                    {
                        var value = propsItem.PropertyInfo.GetValue(entityMetadata);

                        if (value == null)
                        {
                            value = DBNull.Value;
                        }
                        else
                        {
                            var label = (Label)value;

                            value = label.UserLocalizedLabel?.Label;

                            if (value == null)
                            {
                                value = DBNull.Value;
                            }
                        }

                        row.Columns.Add(new InMemoryColumn(propsItem.PropertyInfo.Name, value));
                    }
                    else if (propsItem.Type.IsEnum)
                    {
                        object valueId = null;
                        var value = propsItem.PropertyInfo.GetValue(entityMetadata);

                        if (value == null)
                        {
                            valueId = DBNull.Value;
                            value = DBNull.Value;
                        }
                        else
                        {
                            valueId = Convert.ToInt32(value);
                            value = Convert.ToString(value);
                        }

                        row.Columns.Add(new InMemoryColumn($"{propsItem.PropertyInfo.Name}Id", valueId));

                        row.Columns.Add(new InMemoryColumn(propsItem.PropertyInfo.Name, value));
                    }
                    else
                    {
                        var value = propsItem.PropertyInfo.GetValue(entityMetadata);

                        if (value == null)
                        {
                            value = DBNull.Value;
                        }

                        row.Columns.Add(new InMemoryColumn(propsItem.PropertyInfo.Name, value));
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private class PropItem
        {
            public PropertyInfo PropertyInfo { get; set; }

            public Type Type { get; set; }
        }
    }
}
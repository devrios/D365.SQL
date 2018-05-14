namespace D365.SQL.D365
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using log4net;
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
    }
}
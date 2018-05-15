using System.Collections.Generic;

namespace D365.SQL
{
    public class SqlEngineSettings
    {
        public List<string> SystemSelectFields { get; set; } = new List<string>()
        {
            "$id", "$name",
            "createdon", "createdby",
            "modifiedon", "modifiedby"
        };

        public int? MaxRecordsPerQuery { get; set; } = 250;

        public int? PageSize { get; set; } = 5000;
    }
}

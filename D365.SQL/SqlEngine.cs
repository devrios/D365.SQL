namespace D365.SQL
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;
    using D365;
    using DML.Select;
    using Engine;
    using Engine.Configuration;
    using Engine.Parsers;

    public class SqlEngine
    {
        public SqlEngine(D365Credential credential)
        {
            Credential = credential;
        }

        public DataSet Execute(string sql)
        {
            using (var service = OrganizationService.Create(Credential))
            {
                var parserManager = new ParserManager(Configuration);

                var parseStatementsResult = parserManager.ParseStatements(sql);

                if (parseStatementsResult.Errors.Any())
                {
                    var sb = new StringBuilder();

                    foreach (var error in parseStatementsResult.Errors)
                    {
                        sb.AppendLine($"{error.Message}");
                    }

                    throw new Exception(sb.ToString());
                }

                var statements = parseStatementsResult.Value;

                var dataSet = new DataSet();

                foreach (var statement in statements)
                {
                    if (statement is SelectStatement)
                    {
                        var selectEngine = new SelectEngine(service);

                        var selectStatement = (SelectStatement)statement;

                        var dt = selectEngine.Execute(selectStatement);

                        dataSet.Tables.Add(dt);
                    }
                    else if (statement is UpdateStatement)
                    {
                        throw new NotSupportedException($"'Update' statement is currently not supported.");
                    }
                    else if (statement is DeleteStatement)
                    {
                        throw new NotSupportedException($"'Delete' statement is currently not supported.");
                    }
                }

                return dataSet;
            }
        }

        private ISqlEngineConfiguration Configuration { get; set; } = new SqlEngineConfiguration();

        private D365Credential Credential { get; set; }    
    }
}
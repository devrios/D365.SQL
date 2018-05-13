namespace D365.SQL.Engine.Configuration
{
    using System.Collections.Generic;
    using DML.Select;
    using Parsers;

    internal class SqlEngineStatementConfiguration : ISqlEngineStatementConfiguration<SelectStatement>
    {
        public IEnumerable<IStatementParser<SelectStatement>> Parsers()
        {
            return new IStatementParser<SelectStatement>[]
            {
                new SelectStatementSelectParser(),
                new SelectStatementFromParser(),
                new SelectStatementOrderParser(),
                new SelectStatementWhereParser(),
            };
        }

        public IEnumerable<SqlStatementRule> Rules()
        {
            var rules = new List<SqlStatementRule>();

            rules.Add(new SqlStatementRule()
            {
                Name = "select",
                RequiresNextToken = true,
                Args = true,
                NextTokens = new List<string>() { "from" }
            });

            rules.Add(new SqlStatementRule()
            {
                Name = "from",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { "where", "order by" }
            });

            rules.Add(new SqlStatementRule()
            {
                Name = "where",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { "order by" }
            });

            rules.Add(new SqlStatementRule()
            {
                Name = "order by",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { }
            });

            return rules;
        }
    }
}
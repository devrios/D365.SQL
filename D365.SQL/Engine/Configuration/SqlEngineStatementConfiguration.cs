namespace D365.SQL.Engine.Configuration
{
    using System.Collections.Generic;
    using DML.Select;
    using Parsers;

    internal class SqlEngineStatementConfiguration : ISqlEngineStatementConfiguration<SelectStatement>
    {
        public IEnumerable<IStatementTokenParser<SelectStatement>> Parsers()
        {
            return new IStatementTokenParser<SelectStatement>[]
            {
                new SelectStatementTokenSelectParser(),
                new SelectStatementTokenFromParser(),
                new SelectStatementTokenOrderParser(),
                new SelectStatementTokenWhereParser(),
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
                NextTokens = new List<string>() { "from" },
                EndToken = true // although you can only have raw value or self contained functions
            });

            rules.Add(new SqlStatementRule()
            {
                Name = "from",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { "full", "left", "right", "join", "inner", "where", "order by" },
                EndToken = true
            });

            var joins = new List<string>() { "full", "left", "right", "join", "inner" };
            var joinsNextTokens = new List<string>() { "full", "left", "right", "join", "inner", "where", "order by" };

            foreach (var join in joins)
            {
                rules.Add(new SqlStatementRule()
                {
                    Name = join,
                    RequiresNextToken = false,
                    Args = true,
                    NextTokens = joinsNextTokens,
                    EndToken = true
                });
            }

            rules.Add(new SqlStatementRule()
            {
                Name = "where",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { "group by", "order by" },
                EndToken = true
            });

            rules.Add(new SqlStatementRule()
            {
                Name = "group by",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { "having", "order by" },
                EndToken = false
            });

            rules.Add(new SqlStatementRule()
            {
                Name = "having",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { "order by" },
                EndToken = true
            });

            rules.Add(new SqlStatementRule()
            {
                Name = "order by",
                RequiresNextToken = false,
                Args = true,
                NextTokens = new List<string>() { },
                EndToken = true
            });

            return rules;
        }
    }
}
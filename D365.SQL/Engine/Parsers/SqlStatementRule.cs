namespace D365.SQL.Engine.Parsers
{
    using System.Collections.Generic;

    internal class SqlStatementRule
    {
        public string Name { get; set; }

        public bool Args { get; set; }

        public bool RequiresNextToken { get; set; }

        public List<string> NextTokens { get; set; }
    }
}
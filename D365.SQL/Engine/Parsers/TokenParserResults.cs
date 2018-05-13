namespace D365.SQL.Engine.Parsers
{
    using System.Collections.Generic;

    internal class TokenParserResults
    {
        public TokenParserResults()
        {
            Errors = new List<SqlStatementError>();
        }

        public List<SqlStatementError> Errors { get; set; }
    }
}
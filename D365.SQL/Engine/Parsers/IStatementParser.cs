namespace D365.SQL.Engine.Parsers
{
    internal interface IStatementParser<T>
    {
        string TokenPath { get; }

        TokenParserResults Parse(T selectToken, string args);
    }
}
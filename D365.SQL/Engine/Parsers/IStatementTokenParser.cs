namespace D365.SQL.Engine.Parsers
{
    internal interface IStatementTokenParser<T>
        where T : IStatement
    {
        string TokenPath { get; }

        TokenParserResults Parse(ParseArgs<T> parseArgs);
    }
}
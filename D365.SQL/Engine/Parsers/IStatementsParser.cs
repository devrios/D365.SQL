namespace D365.SQL.Engine.Parsers
{
    using DML.Select;

    internal interface IStatementsParser
    {
        Result<SelectStatement, SqlStatementError> ParseSelect(SqlStatement statement);
    }
}
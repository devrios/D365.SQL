namespace D365.SQL.Engine.Parsers
{
    internal class SqlStatementError
    {
        public SqlStatementError(string message, int positionIndex)
        {
            Message = message;
            PositionIndex = positionIndex;
        }

        public string Message { get; set; }

        public int PositionIndex { get; set; }
    }
}
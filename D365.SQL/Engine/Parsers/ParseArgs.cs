namespace D365.SQL.Engine.Parsers
{
    internal class ParseArgs<T>
        where T : IStatement
    {
        public IStatementsParser StatementsParser { get; set; }

        public T Statement { get; set; }

        public string StatementArgs { get; set; }
    }
}
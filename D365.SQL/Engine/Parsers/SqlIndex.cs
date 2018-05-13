namespace D365.SQL.Engine.Parsers
{
    internal class SqlIndex
    {
        public SqlIndex(int index, int sourceIndex)
        {
            Index = index;
            SourceIndex = sourceIndex;
        }

        public int Index { get; private set; }

        public int SourceIndex { get; private set; }
    }
}
namespace D365.SQL.Tests.Parser.GetWords
{
    public class GetWordsItem
    {
        public GetWordsItem(string sql, params string[] expected)
        {
            Sql = sql;
            Expected = expected;
        }

        public string Sql { get; set; }

        public string[] Expected { get; set; }
    }
}
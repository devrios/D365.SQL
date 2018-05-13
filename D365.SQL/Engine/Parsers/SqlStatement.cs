namespace D365.SQL.Engine.Parsers
{
    using System.Collections.Generic;
    using System.Text;

    internal class SqlStatement
    {
        public SqlStatement()
        {
            Builder = new StringBuilder();
            Indexes = new List<SqlIndex>();
        }
        public string Sql => Builder.ToString();

        public StringBuilder Builder { get; set; }

        public List<SqlIndex> Indexes { get; set; }
    }
}
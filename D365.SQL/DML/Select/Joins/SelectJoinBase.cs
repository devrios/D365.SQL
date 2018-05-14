namespace D365.SQL.DML.Select.Joins
{
    using Columns;
    using Engine.Parsers;

    internal abstract class SelectJoinBase
    {
        public abstract SelectJoinsTypeEnum Type { get; }
    }

    internal class SelectJoinTable
    {
        public string Server { get; set; }

        public string Alias { get; set; }

        public string Table { get; set; }

        public SelectJoinsTypeEnum Type => SelectJoinsTypeEnum.Table;
    }

    internal class TableColumn 
    {
        public TableColumn(string name)
        {
            var parsedName = name.ParseColumn();

            Name = parsedName.Column;
            Alias = parsedName.Alias;
        }

        public string Alias { get; set; }

        public string Name { get; set; }
    }
}
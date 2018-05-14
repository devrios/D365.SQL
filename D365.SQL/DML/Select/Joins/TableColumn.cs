namespace D365.SQL.DML.Select.Joins
{
    using Engine.Parsers;

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
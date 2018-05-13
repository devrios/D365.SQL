namespace D365.SQL.DML.Select.Where
{
    using Columns;
    using Engine.Parsers;

    internal class SelectWhereFieldColumn : SelectWhereColumnBase
    {
        public SelectWhereFieldColumn(string name)
        {
            var parsedName = name.ParseColumn();
            
            Name = parsedName.Column;
            Alias = parsedName.Alias;
        }

        public string Name { get; set; }

        public string Alias { get; set; }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.Field;
    }
}
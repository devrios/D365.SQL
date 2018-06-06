namespace D365.SQL.DML.Select.Columns
{
    using Engine.Parsers;

    internal class FieldSelectColumn : ColumnBase
    {
        public FieldSelectColumn(string name)
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
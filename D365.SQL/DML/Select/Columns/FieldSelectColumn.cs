namespace D365.SQL.DML.Select.Columns
{
    using Engine.Parsers;

    internal class FieldSelectColumn : SelectColumnBase
    {
        public FieldSelectColumn(string name)
            : this(name, name)
        {
        }

        public FieldSelectColumn(string label, string name)
            : base(label)
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
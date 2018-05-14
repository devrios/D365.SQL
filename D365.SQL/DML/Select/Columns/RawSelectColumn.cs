namespace D365.SQL.DML.Select.Columns
{
    internal class RawSelectColumn : SelectColumnBase
    {
        public RawSelectColumn(string label, object value)
            : base(label)
        {
            Value = value;
        }

        public object Value { get; set; }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.Raw;
    }
}
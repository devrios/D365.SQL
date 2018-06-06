namespace D365.SQL.DML.Select.Columns
{
    internal class RawSelectColumn : ColumnBase
    {
        public RawSelectColumn(object value)
        {
            Value = value;
        }

        public object Value { get; set; }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.Raw;
    }
}
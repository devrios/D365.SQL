namespace D365.SQL.DML.Select.Where
{
    using Columns;

    internal class SelectWhereRawColumn : SelectWhereColumnBase
    {
        public SelectWhereRawColumn(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.Raw;
    }
}
namespace D365.SQL.DML.Select.Columns
{
    internal class InnerSelectColumn : SelectColumnBase
    {
        public InnerSelectColumn(string label, SelectStatement selectStatement)
            : base(label)
        {
            SelectStatement = selectStatement;
        }

        public SelectStatement SelectStatement { get; set; }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.InnerSelect;
    }
}
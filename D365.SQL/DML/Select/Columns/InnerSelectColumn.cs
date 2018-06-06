namespace D365.SQL.DML.Select.Columns
{
    internal class InnerSelectColumn : ColumnBase
    {
        public InnerSelectColumn(SelectStatement selectStatement)
        {
            SelectStatement = selectStatement;
        }

        public SelectStatement SelectStatement { get; set; }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.InnerSelect;
    }
}
namespace D365.SQL.DML.Select.Columns
{
    internal class AllSelectColumn : SelectColumnBase
    {
        public AllSelectColumn()
            : base(null)
        {
        }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.All;
    }
}
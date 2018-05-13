namespace D365.SQL.DML.Select.Columns
{
    internal class SystemSelectColumn : SelectColumnBase
    {
        public SystemSelectColumn()
            : base(null)
        {
        }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.System;
    }
}
namespace D365.SQL.DML.Select.Columns
{
    internal class SystemSelectColumn : ColumnBase
    {
        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.System;
    }
}
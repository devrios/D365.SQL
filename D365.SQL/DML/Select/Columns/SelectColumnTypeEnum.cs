namespace D365.SQL.DML.Select.Columns
{
    internal enum SelectColumnTypeEnum
    {
        None = 0,
        All = 1,
        System = 2,
        Field = 3,
        InnerSelect = 10,
        Raw = 100,
    }
}
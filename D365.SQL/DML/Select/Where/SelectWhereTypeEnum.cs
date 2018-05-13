namespace D365.SQL.DML.Select.Where
{
    internal enum SelectWhereTypeEnum
    {
        None = 0,
        Group = 1,
        Operator = 2,
        Comparison = 3,
        Like = 4,
        Between = 5,
        Null = 6,
        In = 7
    }
}
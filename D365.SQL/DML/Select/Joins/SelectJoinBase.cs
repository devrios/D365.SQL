namespace D365.SQL.DML.Select.Joins
{
    using Columns;

    internal abstract class SelectJoinBase
    {
        public abstract SelectJoinsTypeEnum Type { get; }
    }
}
namespace D365.SQL.DML.Select.Where
{
    internal class SelectWhereNull : SelectWhereBase
    {
        public SelectWhereNull(SelectWhereColumnBase expression)
        {
            Expression = expression;
        }

        public bool Negate { get; set; }

        public SelectWhereColumnBase Expression { get; private set; }

        public override SelectWhereTypeEnum Type => SelectWhereTypeEnum.Null;
    }
}
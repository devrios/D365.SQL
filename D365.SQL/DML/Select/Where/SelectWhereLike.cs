namespace D365.SQL.DML.Select.Where
{
    internal class SelectWhereLike : SelectWhereBase
    {
        public SelectWhereLike(SelectWhereColumnBase leftExpression, SelectWhereColumnBase rightExpression)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public bool Negate { get; set; }

        public SelectWhereColumnBase LeftExpression { get; private set; }

        public SelectWhereColumnBase RightExpression { get; private set; }

        public override SelectWhereTypeEnum Type => SelectWhereTypeEnum.Like;
    }
}
namespace D365.SQL.DML.Select.Where
{
    internal class SelectWhereComparison : SelectWhereBase
    {
        public SelectWhereComparison(SelectWhereColumnBase leftExpression, SelectWhereComparisonOperatorEnum @operator, SelectWhereColumnBase rightEpression)
        {
            LeftExpression = leftExpression;
            Operator = @operator;
            RightExpression = rightEpression;
        }

        public bool Negate { get; set; }

        public SelectWhereColumnBase LeftExpression { get; private set; }

        public SelectWhereComparisonOperatorEnum Operator { get; set; }

        public SelectWhereColumnBase RightExpression { get; private set; }

        public override SelectWhereTypeEnum Type => SelectWhereTypeEnum.Comparison;
    }
}
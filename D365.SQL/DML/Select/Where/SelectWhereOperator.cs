namespace D365.SQL.DML.Select.Where
{
    internal class SelectWhereOperator : SelectWhereBase
    {
        public SelectWhereOperator(SelectWhereClauseOperatorEnum clauseOperator)
        {
            ClauseOperator = clauseOperator;
        }

        public SelectWhereClauseOperatorEnum ClauseOperator { get; private set; }

        public override SelectWhereTypeEnum Type => SelectWhereTypeEnum.Operator;
    }
}
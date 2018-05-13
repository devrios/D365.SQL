namespace D365.SQL.DML.Select.Order
{
    internal abstract class SelectOrderBase
    {
        protected SelectOrderBase(OrderDirection direction)
        {
            Direction = direction;
        }

        public OrderDirection Direction { get; set; }

        public abstract SelectOrderTypeEnum Type { get; }
    }
}
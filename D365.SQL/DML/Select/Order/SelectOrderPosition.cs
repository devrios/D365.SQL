namespace D365.SQL.DML.Select.Order
{
    internal class SelectOrderPosition : SelectOrderBase
    {
        public SelectOrderPosition(int position)
            : this(position, OrderDirection.Asc)
        {
            
        }

        public SelectOrderPosition(int position, OrderDirection direction)
            : base(direction)
        {
            Position = position;
        }

        public int Position { get; set; }

        public override SelectOrderTypeEnum Type => SelectOrderTypeEnum.Position;
    }
}
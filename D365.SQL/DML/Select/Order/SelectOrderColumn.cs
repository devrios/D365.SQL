namespace D365.SQL.DML.Select.Order
{
    using Engine.Parsers;

    internal class SelectOrderColumn : SelectOrderBase
    {
        public SelectOrderColumn(string name)
            : this(name, OrderDirection.Asc)
        {

        }

        public SelectOrderColumn(string name, OrderDirection direction)
            : base(direction)
        {
            var parsedName = name.ParseColumn();

            Name = parsedName.Column;
            Alias = parsedName.Alias;
        }

        public string Alias { get; private set; }

        public string Name { get; private set; }

        public override SelectOrderTypeEnum Type => SelectOrderTypeEnum.Column;
    }
}
namespace D365.SQL.DML.Select.From
{
    using Engine.Parsers;

    internal class SelectFrom : SelectFromBase
    {
        public SelectFrom(string name)
            : this(name, null)
        {

        }

        public SelectFrom(string name, string alias)
        {
            var parsedName = name.ParseFrom();

            Name = parsedName.Name;
            Server = parsedName.Server;
            Alias = alias;
        }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Server { get; set; }

        public override SelectFromTypeEnum Type => SelectFromTypeEnum.Table;
    }
}
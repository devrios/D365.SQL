namespace D365.SQL.DML.Select.Joins
{
    internal class SelectJoinTable
    {
        public string Server { get; set; }

        public string Alias { get; set; }

        public string Table { get; set; }

        public SelectJoinsTypeEnum Type => SelectJoinsTypeEnum.Table;
    }
}
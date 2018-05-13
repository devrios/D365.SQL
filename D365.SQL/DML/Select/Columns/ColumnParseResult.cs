namespace D365.SQL.DML.Select.Columns
{
    internal class ColumnParseResult
    {
        public string Alias { get; set; }

        public string Column { get; set; }

        public string Server { get; set; }

        public bool Mixed => string.IsNullOrWhiteSpace(Alias) == false || string.IsNullOrWhiteSpace(Server) == false;
    }
}
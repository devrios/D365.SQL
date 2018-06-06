namespace D365.SQL.DML.Select.Columns
{
    using Common;

    internal sealed class SelectColumn
    {
        public SelectColumn(ColumnBase column, string label)
        {
            if (label.IsNotEmpty())
            {
                Label = label.CleanRaw();
            }

            Column = column;
        }

        public string Label { get; set; }

        public ColumnBase Column { get; set; }
    }
}
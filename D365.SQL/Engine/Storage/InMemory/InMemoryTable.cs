namespace D365.SQL.Engine.Storage.InMemory
{
    using System.Collections.Generic;

    internal class InMemoryTable
    {
        private List<InMemoryTableRow> _rows;
        private List<ColumnInfo> _columns;

        public InMemoryTable(string name, List<ColumnInfo> columns)
        {
            Name = name;
            Columns = columns;
        }

        public string Name { get; set; }

        public List<ColumnInfo> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new List<ColumnInfo>();
                }

                return _columns;
            }
            set
            {
                _columns = value;
            }
        }

        public List<InMemoryTableRow> Rows
        {
            get
            {
                if (_rows == null)
                {
                    _rows = new List<InMemoryTableRow>();
                }

                return _rows;
            }
            set
            {
                _rows = value;
            }
        }
    }
}
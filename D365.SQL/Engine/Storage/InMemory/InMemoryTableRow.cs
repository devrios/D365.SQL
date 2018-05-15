namespace D365.SQL.Engine.Storage.InMemory
{
    using System.Collections.Generic;

    internal class InMemoryTableRow
    {
        private List<InMemoryColumn> _columns;

        public bool Deleted { get; set; }

        public List<InMemoryColumn> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new List<InMemoryColumn>();
                }

                return _columns;
            }
            set
            {
                _columns = value;
            }
        }
    }
}
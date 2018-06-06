namespace D365.SQL.DML.Select.Columns
{
    using System.Collections.Generic;

    internal class FunctionSelectColumn : ColumnBase
    {
        private List<ColumnBase> _args;

        public FunctionSelectColumn(string name)
        {
            Name = name;
        }
        
        public string Name { get; private set; }

        public List<ColumnBase> Args
        {
            get
            {
                if (_args == null)
                {
                    _args = new List<ColumnBase>();
                }

                return _args;
            }
            set
            {
                _args = value;
            }
        }

        public override SelectColumnTypeEnum Type => SelectColumnTypeEnum.Function;
    }
}
namespace D365.SQL.DML.Select.Columns
{
    using System.Collections.Generic;

    internal class SelectColumnFunction : SelectColumnBase
    {
        private List<SelectColumnBase> _args;

        public SelectColumnFunction(string name, string label)
            : base(label)
        {
            Name = name;
        }

        public SelectColumnFunction(string name)
            : this(name, name)
        {
        }

        public string Name { get; private set; }

        public List<SelectColumnBase> Args
        {
            get
            {
                if (_args == null)
                {
                    _args = new List<SelectColumnBase>();
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
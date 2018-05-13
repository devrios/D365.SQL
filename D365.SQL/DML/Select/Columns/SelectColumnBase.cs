namespace D365.SQL.DML.Select.Columns
{
    using System;
    using Common;

    internal abstract class SelectColumnBase
    {
        private Type _valueType;

        protected SelectColumnBase(string label)
        {
            if (label.IsNotEmpty())
            {
                var dotIndex = label.IndexOf('.');

                Label = dotIndex >= 0
                    ? label.Substring(dotIndex + 1)
                    : label;
            }
        }

        public string Label { get; set; }

        public abstract SelectColumnTypeEnum Type { get; }

        public Type ValueType
        {
            get
            {
                if (_valueType == null)
                    return typeof(string);

                return _valueType;
            }
            set
            {
                _valueType = value;
            }
        }
    }
}
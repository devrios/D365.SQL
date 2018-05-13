namespace D365.SQL.DML.Select.Where
{
    using System;
    using Columns;

    internal abstract class SelectWhereColumnBase
    {
        private Type _valueType;

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
namespace D365.SQL.DML.Select.Columns
{
    using System;

    internal abstract class ColumnBase
    {
        private Type _valueType;

        public abstract SelectColumnTypeEnum Type { get; }

        public Type ValueType
        {
            get
            {
                if (_valueType == null)
                    return typeof(object);

                return _valueType;
            }
            set
            {
                _valueType = value;
            }
        }
    }
}
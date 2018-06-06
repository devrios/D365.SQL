namespace D365.SQL.Engine.Parsers
{
    using System.Collections.Generic;

    internal class Result<TValue, TError>
    {
        private List<TError> _errors;

        public Result(TValue value)
        {
            Value = value;
        }

        public Result(List<TError> errors)
        {
            Errors = errors;
        }

        public TValue Value { get; }

        public List<TError> Errors
        {
            get
            {
                return _errors ?? (_errors = new List<TError>());
            }
            set
            {
                _errors = value;
            }
        }
    }
}
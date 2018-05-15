namespace D365.SQL.Engine.Storage.InMemory
{
    internal class InMemoryColumn
    {
        public InMemoryColumn(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public object Value { get; set; }
    }
}
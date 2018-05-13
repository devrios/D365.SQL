namespace D365.SQL.Engine
{
    internal class DataItem
    {
        public DataItem(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public object Value { get; set; }
    }
}
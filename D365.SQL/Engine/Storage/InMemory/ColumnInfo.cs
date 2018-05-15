namespace D365.SQL.Engine.Storage.InMemory
{
    using System;

    internal class ColumnInfo
    {
        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public Type Type { get; }

        public bool IsVisible { get; set; } = true;
    }
}
namespace D365.SQL.Engine.Parsers
{
    internal class TokenSegment
    {
        public TokenSegment(string name, string args)
        {
            Name = name;
            Args = args;
        }
        public string Name { get; set; }
        public string Args { get; set; }
    }
}
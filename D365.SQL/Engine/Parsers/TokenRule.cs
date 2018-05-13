namespace D365.SQL.Engine.Parsers
{
    using System.Collections.Generic;
    using System.Linq;

    internal class TokenRule
    {
        public TokenRule(string name, string[] nextTokens)
        {
            Name = name;
            NextTokens = nextTokens.ToList();
            ChildTokens = new List<TokenRule>();
        }
        public string Name { get; set; }

        public List<string> NextTokens { get; set; }

        public List<TokenRule> ChildTokens { get; set; }
    }
}
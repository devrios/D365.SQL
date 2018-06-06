namespace D365.SQL.Tests.Parser.GetWords
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GetWordsSelectTests : GetWordsTestsBase
    {
        [TestMethod]
        public void TestWords()
        {
            var items = new List<GetWordsItem>()
            {
                new GetWordsItem(""),
                new GetWordsItem(" "),
                new GetWordsItem("  "),

                new GetWordsItem(",", ","),
                new GetWordsItem(" ,", ","),
                new GetWordsItem(", ", ","),

                new GetWordsItem("one", "one"),
                new GetWordsItem("one two", "one", "two"),
                new GetWordsItem("one two three", "one", "two", "three"),

                new GetWordsItem("one order by two", "one", "order by", "two"),
                new GetWordsItem("one group by two", "one", "group by", "two"),
            };

            Expected(items);
        }

        [TestMethod]
        public void TestEqualComparison()
        {
            var items = new List<GetWordsItem>()
            {
                new GetWordsItem("field='1'", "field", "=", "'1'"),
                new GetWordsItem("field ='1'", "field", "=", "'1'"),
                new GetWordsItem("field= '1'", "field", "=", "'1'"),
                new GetWordsItem("field = '1'", "field", "=", "'1'"),

                new GetWordsItem("field=1", "field", "=", "1"),
                new GetWordsItem("field =1", "field", "=", "1"),
                new GetWordsItem("field= 1", "field", "=", "1"),
                new GetWordsItem("field = 1", "field", "=", "1"),

                new GetWordsItem("[field]='1'", "[field]", "=", "'1'"),
                new GetWordsItem("[field] ='1'", "[field]", "=", "'1'"),
                new GetWordsItem("[field]= '1'", "[field]", "=", "'1'"),
                new GetWordsItem("[field] = '1'", "[field]", "=", "'1'"),
            };

            Expected(items);
        }

        [TestMethod]
        public void TestSquareBrackets()
        {
            var items = new List<GetWordsItem>()
            {
                new GetWordsItem("[field]", "[field]"),
                new GetWordsItem("[field]abc", "[field]", "abc"),
                new GetWordsItem("[field],abc", "[field]", ",", "abc"),
                new GetWordsItem("[field] abc", "[field]", "abc"),

                new GetWordsItem("[field[", "[field["),
                new GetWordsItem("[field", "[field"),
                new GetWordsItem("field]", "field]"),
                new GetWordsItem("]field]", "]", "field]"),
                new GetWordsItem("]field[", "]", "field", "["),
                new GetWordsItem("]field", "]", "field"),
                new GetWordsItem("field[", "field", "["),
                
                new GetWordsItem("[fj0439j4f '093j4 f093j4f d3]", "[fj0439j4f '093j4 f093j4f d3]"),
                new GetWordsItem("abc,[field],abc", "abc", ",", "[field]", ",", "abc"),
                new GetWordsItem("abc [field] abc", "abc", "[field]", "abc"),
                new GetWordsItem("abc [field abc", "abc", "[field abc"),
                new GetWordsItem("abc field] abc", "abc", "field]", "abc"),
                new GetWordsItem("abc[field]abc", "abc", "[field]", "abc"),
                new GetWordsItem("abc [ field ] abc", "abc", "[ field ]", "abc"),
                new GetWordsItem("abc [ field abc", "abc", "[ field abc"),
                new GetWordsItem("abc field ] abc", "abc", "field", "]", "abc"),
            };

            Expected(items);
        }
    }
}
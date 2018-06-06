namespace D365.SQL.Tests.Parser.GetWords
{
    using System.Collections.Generic;
    using System.Linq;
    using Engine.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GetWordsColumnsTests : GetWordsTestsBase
    {
        [TestMethod]
        public void SimpleParsing()
        {
            var items = new List<GetWordsItem>()
            {
                new GetWordsItem("'abc' as Col", "'abc'", "as", "Col"),
                new GetWordsItem("'abc' Col", "'abc'", "Col"),
                new GetWordsItem("123 Col", "123", "Col"),
                new GetWordsItem("123 as Col", "123", "as", "Col"),
                new GetWordsItem("123.456 Col", "123.456", "Col"),
                new GetWordsItem("123.456 as Col", "123.456", "as", "Col"),
                new GetWordsItem("abc as Col", "abc", "as", "Col"),
                new GetWordsItem("abc Col", "abc", "Col"),
                new GetWordsItem("abc", "abc"),
                new GetWordsItem("abc as", "abc", "as"),
                new GetWordsItem("1", "1"),
            };

            Expected(items);
        }

        [TestMethod]
        public void AliasParsing()
        {
            var items = new List<GetWordsItem>()
            {
                new GetWordsItem("a.abc", "a.abc"),
                new GetWordsItem("a.[abc]", "a.[abc]"),
                new GetWordsItem("[a.abc]", "[a.abc]"),

                new GetWordsItem("a.abc label", "a.abc", "label"),
                new GetWordsItem("a.[abc] label", "a.[abc]", "label"),
                new GetWordsItem("[a.abc] label", "[a.abc]", "label"),

                new GetWordsItem("a.abc as label", "a.abc", "as", "label"),
                new GetWordsItem("a.[abc] as label", "a.[abc]", "as", "label"),
                new GetWordsItem("[a.abc] as label", "[a.abc]", "as", "label"),
            };

            Expected(items);
        }

        [TestMethod]
        public void Empty()
        {
            var sql = "";
            var words = ParserUtils.GetWords(sql);

            Assert.IsNotNull(words);
            Assert.AreEqual(0, words.ToList().Count);
        }

        [TestMethod]
        public void Null()
        {
            var words = ParserUtils.GetWords(null);

            Assert.IsNotNull(words);
            Assert.AreEqual(0, words.ToList().Count);
        }
    }
}
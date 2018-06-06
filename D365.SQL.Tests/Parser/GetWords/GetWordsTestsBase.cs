namespace D365.SQL.Tests.Parser.GetWords
{
    using System.Collections.Generic;
    using System.Linq;
    using Engine.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class GetWordsTestsBase
    {
        protected void Expected(IEnumerable<string> source, params string[] expected)
        {
            Expected(null, source, expected);
        }

        private void Expected(string prefix, IEnumerable<string> source, params string[] expected)
        {
            var words = source.ToList();
            var expectedString = string.Join(", ", expected);
            var wasString = string.Join(", ", words);

            Assert.AreEqual(expected.Length, words.Count, $"{prefix}\nExpected: {expectedString}\nWas:      {wasString}");

            for (int i = 0; i < words.Count; i++)
            {
                Assert.AreEqual(expected[i], words[i], $"{prefix}\nExpected: {expectedString}\nWas:      {wasString}");
            }
        }

        protected void Expected(List<GetWordsItem> items)
        {
            var index = 0;
            foreach (var item in items)
            {
                var words = ParserUtils.GetWords(item.Sql).ToList();

                Expected($"\nIndex {index++}: {item.Sql}", words, item.Expected);
            }
        }

    }
}
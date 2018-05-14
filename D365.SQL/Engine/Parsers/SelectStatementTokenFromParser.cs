namespace D365.SQL.Engine.Parsers
{
    using System.Linq;
    using DML.Select;
    using DML.Select.From;

    internal class SelectStatementTokenFromParser : IStatementTokenParser<SelectStatement>
    {
        public string TokenPath => "select.from";

        public TokenParserResults Parse(ParseArgs<SelectStatement> parseArgs)
        {
            var selectStatement = parseArgs.Statement;
            var args = parseArgs.StatementArgs;

            var results = new TokenParserResults();

            var words = ParserUtils.GetWords(args).ToList();

            if (words.Any())
            {
                var columns = ParserUtils.SplitTokens(words);

                foreach (var wordList in columns)
                {
                    SelectFromBase selectFrom = null;

                    var startWord = wordList[0];

                    if (wordList.Count > 3)
                    {
                        results.Errors.Add(new SqlStatementError("Unknown tokens", 0));

                        return results;
                    }
                    else if (wordList.Count == 3)
                    {
                        if (wordList[1] != "as")
                        {
                            results.Errors.Add(new SqlStatementError("Expecting 'As' keyword", 0));

                            return results;
                        }

                        selectFrom = new SelectFrom(startWord, wordList[2]);
                    }
                    else if (wordList.Count == 2)
                    {
                        selectFrom = new SelectFrom(startWord, wordList[1]);
                    }
                    else
                    {
                        selectFrom = new SelectFrom(startWord);
                    }

                    if (selectFrom != null)
                    {
                        selectStatement.From.Add(selectFrom);
                    }
                }
            }

            return results;
        }
    }
}
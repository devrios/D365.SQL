namespace D365.SQL.Engine.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common;
    using DML.Select;
    using DML.Select.Columns;

    internal class SelectStatementTokenSelectParser : IStatementTokenParser<SelectStatement>
    {
        public string TokenPath => "select.select";

        public TokenParserResults Parse(ParseArgs<SelectStatement> parseArgs)
        {
            var selectStatement = parseArgs.Statement;
            var args = parseArgs.StatementArgs;
            var statementParser = parseArgs.StatementsParser;

            var results = new TokenParserResults();

            var words = ParserUtils.GetWords(args).ToList();

            var index = 0;

            if (words.Any())
            {
                if (words[0] == "top")
                {
                    if (words.Count >= 2)
                    {
                        var value = 0;

                        if (int.TryParse(words[1], out value))
                        {
                            selectStatement.Top = value;
                            index = 2;
                        }
                        else
                        {
                            results.Errors.Add(new SqlStatementError("Top expects a numerical value", 0));
                        }
                    }
                    else
                    {
                        results.Errors.Add(new SqlStatementError("Top without a numerical value", 0));
                    }
                }

                if (results.Errors.Any())
                {
                    return results;
                }

                var columns = ParserUtils.SplitTokens(words.GetRange(index, words.Count - index));
                
                var parserEngine = new ParserEngine(statementParser);

                foreach (var wordList in columns)
                {
                    if (wordList.Any() == false)
                    {
                        results.Errors.Add(new SqlStatementError("No column specified", 0));

                        return results;
                    }

                    var parseResult = parserEngine.ParseSelectColumn(wordList);

                    if (parseResult == null)
                    {
                        throw new NotSupportedException();
                    }

                    selectStatement.Columns.Add(parseResult.Value);
                }
            }

            return results;
        }
    }
}
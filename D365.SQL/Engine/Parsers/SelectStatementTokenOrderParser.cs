namespace D365.SQL.Engine.Parsers
{
    using System;
    using System.Linq;
    using DML.Select;
    using DML.Select.Order;

    internal class SelectStatementTokenOrderParser : IStatementTokenParser<SelectStatement>
    {
        public string TokenPath => "select.order by";

        public TokenParserResults Parse(ParseArgs<SelectStatement> parseArgs)
        {
            var selectStatement = parseArgs.Statement;
            var args = parseArgs.StatementArgs;

            var results = new TokenParserResults();

            var words = ParserUtils.GetWords(args).ToList();

            var segments = ParserUtils.SplitTokens(words);

            foreach (var wordList in segments)
            {
                if (wordList.Any() == false)
                {
                    results.Errors.Add(new SqlStatementError("No order clause specified", 0));

                    return results;
                }

                SelectOrderBase orderColumn = null;

                var startWord = wordList[0];

                var number = 0;

                if (int.TryParse(startWord, out number))
                {
                    if (wordList.Count > 2)
                    {
                        results.Errors.Add(new SqlStatementError("Unknown tokens", 0));

                        return results;
                    }
                    else if (wordList.Count == 2)
                    {
                        if (wordList[1] != "asc" && wordList[1] != "desc")
                        {
                            results.Errors.Add(new SqlStatementError("Expecting 'Asc' or 'Desc' keyword", 0));

                            return results;
                        }

                        var sortDirection = wordList[1] == "asc" ? OrderDirection.Asc : OrderDirection.Desc;

                        orderColumn = new SelectOrderPosition(number, sortDirection);
                    }
                    else if (wordList.Count == 1)
                    {
                        orderColumn = new SelectOrderPosition(number);
                    }
                }
                else if (startWord.StartsWith("'") && startWord.EndsWith("'"))
                {
                    results.Errors.Add(new SqlStatementError("A constant expression was encountered in the ORDER BY list", 0));

                    return results;
                }
                else if (char.IsLetter(startWord[0]))
                {
                    if (wordList.Count > 2)
                    {
                        results.Errors.Add(new SqlStatementError("Unknown tokens", 0));

                        return results;
                    }
                    else if (wordList.Count == 2)
                    {
                        if (wordList[1] != "asc" && wordList[1] != "desc")
                        {
                            results.Errors.Add(new SqlStatementError("Expecting 'Asc' or 'Desc' keyword", 0));

                            return results;
                        }

                        var sortDirection = wordList[1] == "asc" ? OrderDirection.Asc : OrderDirection.Desc;

                        orderColumn = new SelectOrderColumn(startWord, sortDirection);
                    }
                    else if (wordList.Count == 1)
                    {
                        orderColumn = new SelectOrderColumn(startWord);
                    }
                }

                if (orderColumn == null)
                {
                    throw new NotSupportedException();
                }

                selectStatement.Order.Add(orderColumn);
            }

            return results;
        }
    }
}
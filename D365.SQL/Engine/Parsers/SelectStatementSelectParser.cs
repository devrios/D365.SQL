namespace D365.SQL.Engine.Parsers
{
    using System;
    using System.Linq;
    using DML.Select;
    using DML.Select.Columns;

    internal class SelectStatementSelectParser : IStatementParser<SelectStatement>
    {
        public string TokenPath => "select.select";

        public TokenParserResults Parse(SelectStatement selectStatement, string args)
        {
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

                var colIndex = 0;

                foreach (var wordList in columns)
                {
                    if (wordList.Any() == false)
                    {
                        results.Errors.Add(new SqlStatementError("No column specified", 0));

                        return results;
                    }

                    SelectColumnBase selectColumn = null;

                    var startWord = wordList[0];

                    var number = 0;

                    if ((startWord.StartsWith("'") && startWord.EndsWith("'")) || (int.TryParse(startWord, out number)))
                    {
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

                            selectColumn = new RawSelectColumn(wordList[2], startWord.Substring(1, startWord.Length - 2));
                        }
                        else if (wordList.Count == 2)
                        {
                            selectColumn = new RawSelectColumn(wordList[1], startWord);
                        }
                        else
                        {
                            selectColumn = new RawSelectColumn($"Col{++colIndex}", startWord.Substring(1, startWord.Length - 2));
                        }
                    }
                    else if (startWord.StartsWith("*"))
                    {
                        if (wordList.Count > 1)
                        {
                            results.Errors.Add(new SqlStatementError("Found unsupported tokens after '*'", 0));

                            return results;
                        }

                        selectColumn = new AllSelectColumn();
                    }
                    else if (startWord.StartsWith("+"))
                    {
                        if (wordList.Count > 1)
                        {
                            results.Errors.Add(new SqlStatementError("Found unsupported tokens after '+'", 0));

                            return results;
                        }

                        selectColumn = new SystemSelectColumn();
                    }
                    else if (char.IsLetter(startWord[0]))
                    {
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

                            selectColumn = new FieldSelectColumn(wordList[2], startWord);
                        }
                        else if (wordList.Count == 2)
                        {
                            selectColumn = new FieldSelectColumn(wordList[1], startWord);
                        }
                        else
                        {
                            selectColumn = new FieldSelectColumn(startWord);
                        }
                    }

                    if (selectColumn == null)
                    {
                        throw new NotSupportedException();
                    }

                    selectStatement.Columns.Add(selectColumn);
                }
            }

            return results;
        }
    }
}
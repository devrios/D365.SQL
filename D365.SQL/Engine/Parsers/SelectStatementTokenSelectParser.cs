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
                
                foreach (var wordList in columns)
                {
                    if (wordList.Any() == false)
                    {
                        results.Errors.Add(new SqlStatementError("No column specified", 0));

                        return results;
                    }

                    SelectColumnBase selectColumn = null;

                    var startWord = wordList[0];

                    double number = 0;

                    if (startWord.IsQuoted() || double.TryParse(startWord, out number))
                    {
                        object value = startWord.IsQuoted()
                            ? startWord.CleanRaw()
                            : number.ToString();

                        var valueType = startWord.IsQuoted()
                            ? typeof(string)
                            : typeof(double);

                        if (startWord.IsQuoted() == false && startWord.IndexOf(".", StringComparison.Ordinal) < 0)
                        {
                            valueType = typeof(int);
                            value = int.Parse(startWord);
                        }

                        if (wordList.Count > 3)
                        {
                            results.Errors.Add(new SqlStatementError("Unknown tokens", 0));

                            return results;
                        }

                        if (wordList.Count == 3)
                        {
                            if (wordList[1] != "as")
                            {
                                results.Errors.Add(new SqlStatementError("Expecting 'As' keyword", 0));

                                return results;
                            }

                            selectColumn = new RawSelectColumn(wordList[2], value)
                            {
                                ValueType = valueType
                            };
                        }
                        else if (wordList.Count == 2)
                        {
                            selectColumn = new RawSelectColumn(wordList[1], value)
                            {
                                ValueType = valueType
                            };
                        }
                        else
                        {
                            selectColumn = new RawSelectColumn(null, value)
                            {
                                ValueType = valueType
                            };
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
                        selectColumn = ParseColumn(wordList, results.Errors);
                    }
                    else if (startWord == "(")
                    {
                        var nextWord = wordList[1];

                        if (nextWord == "select")
                        {
                            var startIndex = 1;
                            var endIndex = wordList.FindLastIndex(x => x == ")");
                            var innerSql = string.Join(" ", wordList.GetRange(startIndex, endIndex - startIndex));

                            var statement = statementParser.ParseSelect(new SqlStatement()
                            {
                                Builder = new StringBuilder(innerSql)
                            });

                            if (statement.Errors.Any())
                            {
                                throw new Exception($"Unable to parse inner select statement '{innerSql}'");
                            }

                            var descTokenIndex = wordList.Count - 1;

                            var label = wordList[descTokenIndex];

                            descTokenIndex--;

                            if (wordList[descTokenIndex].NotIn(")", "as"))
                            {
                                throw new Exception($"Unexpected token after inner select '{innerSql}'");
                            }

                            if (wordList[descTokenIndex] == "as")
                            {
                                descTokenIndex--;
                            }

                            if (wordList[descTokenIndex] != ")")
                            {
                                throw new Exception($"Unexpected token after inner select '{innerSql}'");
                            }

                            selectColumn = new InnerSelectColumn(label, statement.Value);
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

        private SelectColumnBase ParseColumn(List<string> wordList, List<SqlStatementError> errors)
        {
            var startWord = wordList[0];
            var ascTokenIndex = 0;
            var descTokenIndex = wordList.Count - 1;
            var nextToken = wordList.Count > 1
                ? wordList[++ascTokenIndex]
                : null;

            if (nextToken == "(")
            {
                var functionName = startWord;
                var functionLabel = "";

                if (wordList[descTokenIndex] == ")")
                {
                    functionLabel = startWord;
                    descTokenIndex--;
                }
                else
                {
                    functionLabel = wordList[descTokenIndex];

                    descTokenIndex--;

                    if (wordList[descTokenIndex].NotIn(")", "as"))
                    {
                        throw new Exception($"Unexpected token after function '{wordList[wordList.Count - 2]}'");
                    }

                    if (wordList[descTokenIndex] == "as")
                    {
                        descTokenIndex--;
                    }

                    if (wordList[descTokenIndex] == ")")
                    {
                        descTokenIndex--;
                    }
                }

                var functionColumn = new SelectColumnFunction(functionName, functionLabel);

                var argList = wordList.GetRange(2, descTokenIndex - 1);

                var functionArgs = ParserUtils.SplitTokens(argList);

                foreach (var functionArg in functionArgs)
                {
                    functionColumn.Args.Add(ParseColumn(functionArg, errors));
                }

                return functionColumn;
            }

            double number = 0;
            SelectColumnBase selectColumn = null;

            if (startWord.IsQuoted() || (double.TryParse(startWord, out number)))
            {
                object value = startWord.IsQuoted()
                    ? startWord.CleanRaw()
                    : number.ToString();

                var valueType = startWord.IsQuoted()
                    ? typeof(string)
                    : typeof(double);

                if (startWord.IsQuoted() == false && startWord.IndexOf(".", StringComparison.Ordinal) < 0)
                {
                    valueType = typeof(int);
                    value = int.Parse(startWord);
                }

                if (wordList.Count > 3)
                {
                    errors.Add(new SqlStatementError("Unknown tokens", 0));

                    return null;
                }

                if (wordList.Count == 3)
                {
                    if (wordList[1] != "as")
                    {
                        errors.Add(new SqlStatementError("Expecting 'As' keyword", 0));

                        return null;
                    }

                    selectColumn = new RawSelectColumn(wordList[2], value)
                    {
                        ValueType = valueType
                    };
                }
                else if (wordList.Count == 2)
                {
                    selectColumn = new RawSelectColumn(wordList[1], value)
                    {
                        ValueType = valueType
                    };
                }
                else
                {
                    selectColumn = new RawSelectColumn(null, value)
                    {
                        ValueType = valueType
                    };
                }

                return selectColumn;
            }

            if (char.IsLetter(startWord[0]))
            {
                if (wordList.Count > 3)
                {
                    errors.Add(new SqlStatementError("Unknown tokens", 0));

                    return null;
                }

                if (wordList.Count == 3)
                {
                    if (wordList[1] != "as")
                    {
                        errors.Add(new SqlStatementError("Expecting 'As' keyword", 0));

                        return null;
                    }

                    selectColumn = new FieldSelectColumn(wordList[2], startWord);
                }
                else if (wordList.Count == 2)
                {
                    selectColumn = new FieldSelectColumn(wordList[1], startWord);
                }
                else
                {
                    selectColumn = new FieldSelectColumn(null, startWord);
                }

                return selectColumn;
            }

            throw new Exception("Unexpected function arguments.");
        }
    }
}
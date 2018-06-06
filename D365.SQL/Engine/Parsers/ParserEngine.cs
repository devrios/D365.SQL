using System;
using System.Collections.Generic;
using System.Text;

namespace D365.SQL.Engine.Parsers
{
    using System.Globalization;
    using System.Linq;
    using Common;
    using DML.Select.Columns;

    internal class ParserEngine
    {
        private readonly IStatementsParser _statementParser;

        public ParserEngine(IStatementsParser statementParser)
        {
            _statementParser = statementParser;
        }

        public Result<SelectColumn, SqlStatementError> ParseSelectColumn(List<string> wordList)
        {
            var result = ParseColumnImpl(wordList, true);

            if (result.Errors.Any())
            {
                return new Result<SelectColumn, SqlStatementError>(result.Errors);
            }

            return new Result<SelectColumn, SqlStatementError>(new SelectColumn(result.Value, result.Label));
        }

        public Result<ColumnBase, SqlStatementError> ParseColumn(List<string> wordList)
        {
            return ParseColumnImpl(wordList, false);
        }

        private ParseColumnResult ParseColumnImpl(List<string> wordList, bool allowLabel)
        {
            var errors = new List<SqlStatementError>();
            var startWord = wordList[0];

            ColumnBase selectColumn = null;
            string label = null;

            decimal number = 0;

            //

            if (startWord.IsQuoted() || decimal.TryParse(startWord, out number))
            {
                object value = startWord.IsQuoted()
                    ? startWord.CleanRaw()
                    : number.ToString();

                var valueType = typeof(object);

                if (startWord.IsQuoted())
                {
                    valueType = typeof(string);

                    var valueString = value.ToString();

                    if (char.IsNumber(valueString[0]) && char.IsNumber(valueString[valueString.Length - 1]))
                    {
                        DateTime date;

                        if (valueString.Length == 10)
                        {
                            if (DateTime.TryParseExact(valueString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            {
                                valueType = typeof(DateTime);
                                value = date;
                            }
                        }
                        else if (valueString.Length == 16)
                        {
                            if (DateTime.TryParseExact(valueString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            {
                                valueType = typeof(DateTime);
                                value = date;
                            }
                        }
                        else if (valueString.Length == 19)
                        {
                            if (DateTime.TryParseExact(valueString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            {
                                valueType = typeof(DateTime);
                                value = date;
                            }
                        }
                    }
                }
                else
                {
                    if (startWord.IndexOf(".", StringComparison.Ordinal) < 0)
                    {
                        valueType = typeof(int);
                        value = int.Parse(startWord);
                    }
                    else
                    {
                        valueType = typeof(double);
                        value = double.Parse(startWord, CultureInfo.InvariantCulture);
                    }
                }

                if (wordList.Count > 3)
                {
                    errors.Add(new SqlStatementError("Unknown tokens", 0));

                    return new ParseColumnResult(errors);
                }

                if (wordList.Count == 3)
                {
                    if (string.Equals(wordList[1], "as", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        errors.Add(new SqlStatementError("Expecting 'As' keyword", 0));

                        return new ParseColumnResult(errors);
                    }

                    label = wordList[2];

                    selectColumn = new RawSelectColumn(value)
                    {
                        ValueType = valueType
                    };
                }
                else if (wordList.Count == 2)
                {
                    label = wordList[1];

                    selectColumn = new RawSelectColumn(value)
                    {
                        ValueType = valueType
                    };
                }
                else
                {
                    selectColumn = new RawSelectColumn(value)
                    {
                        ValueType = valueType
                    };
                }
            }
            else if (startWord.StartsWith("*"))
            {
                if (wordList.Count > 1)
                {
                    errors.Add(new SqlStatementError("Found unsupported tokens after '*'", 0));

                    return new ParseColumnResult(errors);
                }

                selectColumn = new AllSelectColumn();
            }
            else if (startWord.StartsWith("+"))
            {
                if (wordList.Count > 1)
                {
                    errors.Add(new SqlStatementError("Found unsupported tokens after '+'", 0));

                    return new ParseColumnResult(errors);
                }

                selectColumn = new SystemSelectColumn();
            }
            else if (char.IsLetter(startWord[0]) || startWord.IsSquareBrackets())
            {
                var ascTokenIndex = 0;
                var descTokenIndex = wordList.Count - 1;
                var nextToken = wordList.Count > 1
                    ? wordList[++ascTokenIndex]
                    : null;

                if (startWord.IsSquareBrackets() == false && nextToken == "(")
                {
                    // Function Liekly

                    var functionName = startWord;
                    var functionLabel = "";

                    if (wordList[descTokenIndex] == ")")
                    {
                        descTokenIndex--;
                    }
                    else
                    {
                        functionLabel = wordList[descTokenIndex];

                        descTokenIndex--;

                        if (wordList[descTokenIndex].ToLower().NotIn(")", "as"))
                        {
                            throw new Exception($"Unexpected token after function '{wordList[wordList.Count - 2]}'");
                        }

                        if (string.Equals(wordList[descTokenIndex], "as", StringComparison.OrdinalIgnoreCase))
                        {
                            descTokenIndex--;
                        }

                        if (wordList[descTokenIndex] == ")")
                        {
                            descTokenIndex--;
                        }
                    }

                    label = functionLabel;

                    var functionColumn = new FunctionSelectColumn(functionName);

                    var argList = wordList.GetRange(2, descTokenIndex - 1);

                    var functionArgs = ParserUtils.SplitTokens(argList);

                    foreach (var functionArg in functionArgs)
                    {
                        var result = ParseColumnImpl(functionArg, false);

                        if (result.Errors.Any())
                        {
                            return result;
                        }

                        functionColumn.Args.Add(result.Value);
                    }

                    selectColumn = functionColumn;
                }
                else
                {
                    if (wordList.Count > 3)
                    {
                        errors.Add(new SqlStatementError("Unknown tokens", 0));

                        return new ParseColumnResult(errors);
                    }

                    if (wordList.Count == 3)
                    {
                        if (string.Equals(wordList[1], "as", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            errors.Add(new SqlStatementError("Expecting 'As' keyword", 0));

                            return new ParseColumnResult(errors);
                        }

                        label = wordList[2];

                        selectColumn = new FieldSelectColumn(startWord);
                    }
                    else if (wordList.Count == 2)
                    {
                        label = wordList[1];

                        selectColumn = new FieldSelectColumn(startWord);
                    }
                    else
                    {
                        selectColumn = new FieldSelectColumn(startWord);
                    }
                }
            }
            else if (startWord == "(")
            {
                // Nested query

                var nextWord = wordList[1];

                if (nextWord == "select")
                {
                    var startIndex = 1;
                    var endIndex = wordList.FindLastIndex(x => x == ")");
                    var innerSql = string.Join(" ", wordList.GetRange(startIndex, endIndex - startIndex));

                    var statement = _statementParser.ParseSelect(new SqlStatement()
                    {
                        Builder = new StringBuilder(innerSql)
                    });

                    if (statement.Errors.Any())
                    {
                        throw new Exception($"Unable to parse inner select statement '{innerSql}'");
                    }

                    var descTokenIndex = wordList.Count - 1;

                    label = wordList[descTokenIndex];

                    descTokenIndex--;

                    if (wordList[descTokenIndex].ToLower().NotIn(")", "as"))
                    {
                        throw new Exception($"Unexpected token after inner select '{innerSql}'");
                    }

                    if (string.Equals(wordList[descTokenIndex], "as", StringComparison.OrdinalIgnoreCase))
                    {
                        descTokenIndex--;
                    }

                    if (wordList[descTokenIndex] != ")")
                    {
                        throw new Exception($"Unexpected token after inner select '{innerSql}'");
                    }

                    selectColumn = new InnerSelectColumn(statement.Value);
                }
            }

            if (selectColumn == null)
            {
                throw new Exception("Unexpected function arguments.");
            }

            if (allowLabel == false && label.IsNotEmpty())
            {
                errors.Add(new SqlStatementError("Unexpected label", 0));

                return new ParseColumnResult(errors);
            }

            return new ParseColumnResult(selectColumn, label);
        }

        #region ParseColumnResult

        private sealed class ParseColumnResult : Result<ColumnBase, SqlStatementError>
        {
            public string Label { get; set; }

            public ParseColumnResult(ColumnBase column, string label)
                : this(column)
            {
                Label = label;
            }

            private ParseColumnResult(ColumnBase value)
                : base(value)
            {
            }

            public ParseColumnResult(List<SqlStatementError> errors)
                : base(errors)
            {
            }
        }

        #endregion
    }
}
namespace D365.SQL.Engine.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Configuration;

    internal class ParserManager
    {
        public ParserManager(ISqlEngineConfiguration configuration)
        {
            Configuration = configuration;
        }

        private ISqlEngineConfiguration Configuration { get; set; }

        public Result<List<IStatement>, SqlStatementError> ParseStatements(string sql)
        {
            var statementsParser = new ParseStatements(Configuration);
           var errors = new List<SqlStatementError>();

           var statementsResult = SplitIntoStatements(sql);

            var parsedStatements = new List<IStatement>();

            foreach (var statement in statementsResult.Value)
            {
                var startWord = statement.Sql.Substring(0, statement.Sql.IndexOf(' '));

                if (string.Equals(startWord, "select"))
                {
                    var selectParseResult = statementsParser.ParseSelect(statement);

                    if (selectParseResult.Errors.Any())
                    {
                        errors.AddRange(selectParseResult.Errors);

                        break;
                    }

                    parsedStatements.Add(selectParseResult.Value);
                }
                else
                {
                    errors.Add(new SqlStatementError($"Statement '{startWord}' is currently not supported.", 0));

                    break;
                }
            }

            var result = new Result<List<IStatement>, SqlStatementError>(parsedStatements)
            {
                Errors = errors
            };

            return result;
        }

        internal Result<List<SqlStatement>, SqlStatementError> SplitIntoStatements(string sql)
        {
            var statements = new List<SqlStatement>();
            var errors = new List<SqlStatementError>();

            var inInlineComment = false;
            var inMultilineComment = false;
            var inQuotes = false;
            var parenthesesCount = 0;
            var trimStart = true;
            var sourceIndex = -1;
            var index = -1;
            var keywords = new List<string>()
            {
                "select", "as", "order", "by", "asc", "desc", "group", "by",
                "left", "right", "inner", "outer", "join", "on",
                "delete",
                "update", "set",
                "from",
                "where", "or", "and", "in", "between", "like"
            };

            var initStatementKeywords = new List<string>()
            {
                "select", "update", "delete"
            };

            var statement = new SqlStatement();

            Action newStatement = () =>
            {
                if (string.IsNullOrWhiteSpace(statement.Sql) == false)
                {
                    var sbFormatted = new StringBuilder();

                    foreach (var word in ParserUtils.GetWords(statement.Sql))
                    {
                        if (keywords.Any(keyword => string.Equals(keyword, word, StringComparison.OrdinalIgnoreCase)))
                        {
                            sbFormatted.Append(word.ToLowerInvariant());
                        }
                        else
                        {
                            sbFormatted.Append(word);
                        }
                        sbFormatted.Append(" ");
                    }

                    statement.Builder = sbFormatted;

                    statements.Add(statement);
                }

                statement = new SqlStatement();

                index = -1;
                trimStart = true;
            };

            for (int i = 0; i < sql.Length; i++)
            {
                sourceIndex++;
                index++;

                var c = sql[i];

                if (trimStart)
                {
                    if (c == ' ')
                    {
                        index--;
                        continue;
                    }
                    trimStart = false;
                }

                if (inMultilineComment)
                {
                    if (c == '*')
                    {
                        var str = sql.Substring(i, 2);

                        if (str == "*/")
                        {
                            inMultilineComment = false;
                            i++;
                            sourceIndex++;
                            index--;
                        }

                        continue;
                    }
                }
                else
                {
                    if (inQuotes == false)
                    {
                        if (c == '\n')
                        {
                            inInlineComment = false;
                            statement.Builder.Append(' ');
                            continue;
                        }

                        if (inInlineComment)
                        {
                            continue;
                        }

                        if (c == '/')
                        {
                            var str = sql.Substring(i, 2);

                            if (str == "/*")
                            {
                                inMultilineComment = true;
                                continue;
                            }

                            if (str == "//")
                            {
                                inInlineComment = true;
                                continue;
                            }
                        }
                    }

                }

                if (inMultilineComment)
                {
                    index--;
                    continue;
                }

                if (c == '\'')
                {
                    if (i == 0 || sql[i - 1] != '\\') // ignore if escaped
                    {
                        inQuotes = !inQuotes;
                    }
                }

                if (c == '(')
                {
                    parenthesesCount++;
                }

                if (c == ')')
                {
                    parenthesesCount--;

                    statement.Builder.Append(c);

                    continue;
                }

                if (parenthesesCount > 0)
                {
                    statement.Builder.Append(c);

                    continue;
                }

                if (c == ';')
                {
                    newStatement();

                    continue;
                }

                if (c == ' ' && ((i > 0 && sql[i - 1] == ' ') || (statement.Sql[statement.Sql.Length - 1] == ' ')))
                {
                    index--;
                    continue;
                }

                if (c == '\t')
                {
                    index--;
                    continue;
                }

                // if we reached here, we are not in a comment or string

                if (inQuotes == false && char.IsLetter(c))
                {
                    foreach (var keyword in initStatementKeywords)
                    {
                        if (i + keyword.Length > sql.Length)
                        {
                            continue;
                        }

                        var word = sql.Substring(i, keyword.Length);

                        if (string.Equals(word, keyword))
                        {
                            newStatement();

                            continue;
                        }
                    }
                }

                statement.Builder.Append(c);

                if (index != sourceIndex)
                {
                    //statement.Indexes.Add(new SqlIndex(index, sourceIndex));	
                }
            }

            if (statement != null && string.IsNullOrWhiteSpace(statement.Sql) == false)
            {
                newStatement();
            }

            var result = new Result<List<SqlStatement>, SqlStatementError>(statements)
            {
                Errors = errors
            };

            return result;
        }        
    }
}
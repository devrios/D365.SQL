namespace D365.SQL.Engine.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Configuration;
    using DML.Select;

    internal class ParserManager
    {
        public ParserManager(ISqlEngineConfiguration configuration)
        {
            Configuration = configuration;
        }

        private ISqlEngineConfiguration Configuration { get; set; }

        public Result<List<IStatement>, SqlStatementError> ParseStatements(string sql)
        {
           var errors = new List<SqlStatementError>();

           var statementsResult = SplitIntoStatements(sql);

            var parsedStatements = new List<IStatement>();

            foreach (var statement in statementsResult.Value)
            {
                var startWord = statement.Sql.Substring(0, statement.Sql.IndexOf(' '));

                if (string.Equals(startWord, "select"))
                {
                    var selectParseResult = ParseSelectStatement(statement);

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

        private Result<List<SqlStatement>, SqlStatementError> SplitIntoStatements(string sql)
        {
            var statements = new List<SqlStatement>();
            var errors = new List<SqlStatementError>();

            var inInlineComment = false;
            var inMultilineComment = false;
            var inQuotes = false;
            var trimStart = true;
            var sourceIndex = -1;
            var index = -1;
            var keywords = new List<string>()
            {
                "select", "as", "order", "by", "asc", "desc", "group", "by",
                "delete",
                "update", "set",
                "from",
                "where", "or", "and", "in", "between", "like"
            };
            var metadata = new List<string>() {
                "contact"
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
                        if (keywords.Any(keyword => string.Equals(keyword, word, StringComparison.OrdinalIgnoreCase)) ||
                            metadata.Any(keyword => string.Equals(keyword, word, StringComparison.OrdinalIgnoreCase)))
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

                if (inMultilineComment || inInlineComment)
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

        private Result<SelectStatement, SqlStatementError> ParseSelectStatement(SqlStatement statement)
        {
            var selectStatement = new SelectStatement();

            var errors = new List<SqlStatementError>();

            var rules = Configuration.SelectStatementConfiguration.Rules();

            var args = new Dictionary<string, string>();
            var words = ParserUtils.GetWords(statement.Sql).ToList();

            if (words.First() != "select")
            {
                errors.Add(new SqlStatementError("No 'Select' statement found.", 0));
            }
            else
            {
                var currentIndex = 0;

                while (true)
                {
                    var word = words[currentIndex];

                    var rule = rules.SingleOrDefault(x => x.Name == word);

                    if (rule == null) break;

                    if (rule.NextTokens.Any())
                    {
                        int? nextTokenIndex = null;

                        foreach (var nextToken in rule.NextTokens)
                        {
                            nextTokenIndex = FindInArray(words, currentIndex, nextToken);

                            if (nextTokenIndex.HasValue) break;
                        }

                        if (rule.RequiresNextToken && nextTokenIndex.HasValue == false)
                        {
                            errors.Add(new SqlStatementError($"Incomplete statement after {word}", 0));
                            break;
                        }

                        if (rule.Args)
                        {
                            var count = nextTokenIndex.HasValue
                                ? nextTokenIndex.Value - currentIndex - 1
                                : words.Count - currentIndex - 1;

                            var tokenArgs = string.Join(" ", words.GetRange(currentIndex + 1, count));

                            if (string.IsNullOrWhiteSpace(tokenArgs))
                            {
                                errors.Add(new SqlStatementError($"Expected arguments after {word}", 0));
                                break;
                            }

                            args.Add(word, tokenArgs);
                        }

                        currentIndex = nextTokenIndex ?? words.Count - 1;
                    }
                    else
                    {
                        if (rule.Args)
                        {
                            var tokenArgs = string.Join(" ", words.GetRange(currentIndex + 1, words.Count - currentIndex - 1));

                            if (string.IsNullOrWhiteSpace(tokenArgs))
                            {
                                errors.Add(new SqlStatementError($"Expected arguments after {word}", 0));
                                break;
                            }

                            args.Add(word, tokenArgs);

                            break;
                        }
                    }
                }
            }

            foreach (var token in args.Keys)
            {
                var tokenPath = "select." + token;

                var parsers = Configuration.SelectStatementConfiguration.Parsers().Where(x => string.Equals(x.TokenPath, tokenPath));

                foreach (var parser in parsers)
                {
                    var parseResults = parser.Parse(selectStatement, args[token]);

                    if (parseResults.Errors.Any())
                    {
                        errors.AddRange(parseResults.Errors);
                    }
                }
            }

            var result = new Result<SelectStatement, SqlStatementError>(selectStatement)
            {
                Errors = errors
            };

            return result;
        }

        /// <summary>
        /// Finds a word in the supplied array
        /// </summary>
        /// <param name="words"></param>
        /// <param name="startIndex"></param>
        /// <param name="word"></param>
        /// <returns>
        /// The index in the array where the first word exists, null otherwise.
        /// </returns>
        private int? FindInArray(List<string> words, int startIndex, string word)
        {
            for (int i = startIndex + 1; i < words.Count; i++)
            {
                if (words[i] == word)
                {
                    return i;
                }
            }

            return null;
        }
    }
}
namespace D365.SQL.Engine.Parsers
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using DML.Select;

    internal sealed class ParseStatements : IStatementsParser
    {
        public ParseStatements(ISqlEngineConfiguration configuration)
        {
            Configuration = configuration;
        }

        private ISqlEngineConfiguration Configuration { get; set; }

        public Result<SelectStatement, SqlStatementError> ParseSelect(SqlStatement statement)
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
                    var parseArgs = new ParseArgs<SelectStatement>()
                    {
                        Statement = selectStatement,
                        StatementArgs = args[token],
                        StatementsParser = this
                    };

                    var parseResults = tokenPath == "select.select"
                        ? new SelectStatementTokenSelectParser().Parse(parseArgs)
                        : parser.Parse(parseArgs);

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
        private int? FindInArray(List<string> words, int startIndex, string findWord)
        {
            var parenthesesCount = 0;

            for (int i = startIndex + 1; i < words.Count; i++)
            {
                var word = words[i];

                if (word == "(")
                {
                    parenthesesCount++;
                    continue;
                }

                if (word == ")")
                {
                    parenthesesCount--;
                    continue;
                }

                if (parenthesesCount > 0)
                {
                    continue;
                }

                if (word == findWord)
                {
                    return i;
                }
            }

            return null;
        }
    }
}
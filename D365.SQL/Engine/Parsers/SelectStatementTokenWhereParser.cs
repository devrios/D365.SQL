namespace D365.SQL.Engine.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using DML.Select;
    using DML.Select.Where;

    internal class SelectStatementTokenWhereParser : IStatementTokenParser<SelectStatement>
    {
        public string TokenPath => "select.where";

        public static Func<string, IEnumerable<string>> GetWords;

        public TokenParserResults Parse(ParseArgs<SelectStatement> parseArgs)
        {
            var selectStatement = parseArgs.Statement;
            var args = parseArgs.StatementArgs;

            var results = new TokenParserResults();

            var words = GetWords != null
                ? GetWords(args).ToList()
                : ParserUtils.GetWords(args).ToList();
            
            if (words.Any())
            {
                var currentGroup = new SelectWhereGroup();
                var groupStack = new Stack<SelectWhereGroup>();
                var clauseWords = new List<string>();

                for (int i = 0; i < words.Count; i++)
                {
                    var word = words[i];

                    if (word.In("(", ")", "and", "or"))
                    {
                        ParseClauseWords(clauseWords, currentGroup);
                    }

                    if (word == "(")
                    {
                        groupStack.Push(currentGroup);

                        var newGroup = new SelectWhereGroup();

                        currentGroup.Clauses.Add(newGroup);

                        currentGroup = newGroup;
                    }
                    else if (word == ")")
                    {
                        currentGroup = groupStack.Pop();
                    }
                    else if (word == "or")
                    {
                        var clauseOperator = new SelectWhereOperator(SelectWhereClauseOperatorEnum.Or);

                        currentGroup.Clauses.Add(clauseOperator);
                    }
                    else if (word == "and")
                    {
                        var clauseOperator = new SelectWhereOperator(SelectWhereClauseOperatorEnum.And);

                        currentGroup.Clauses.Add(clauseOperator);
                    }
                    else
                    {
                        clauseWords.Add(word);
                    }
                }

                ParseClauseWords(clauseWords, currentGroup);

                selectStatement.Where = currentGroup.Clauses;
            }

            return results;
        }

        private static void ParseClauseWords(List<string> clauseWords, SelectWhereGroup currentGroup)
        {
            if (clauseWords.Any())
            {
                var negate = false;
                var index = 0;
                var startWord = clauseWords[index];

                if (string.Equals(startWord, "select"))
                {
                    // todo recursion select statement
                    throw new NotSupportedException("Inner Select not currently supported.");
                }

                var leftExpression = ParseWhereColumn(startWord);

                var nextWord = clauseWords[++index];

                if (nextWord == "not")
                {
                    negate = true;

                    nextWord = clauseWords[++index];
                }

                if (nextWord.In("=", "<", ">", "!"))
                {
                    var comparison = "";

                    for (; index < clauseWords.Count - 1; index++)
                    {
                        comparison += clauseWords[index];
                    }

                    var comparisonOperator = SelectWhereComparisonOperatorEnum.None;

                    switch (comparison)
                    {
                        case "=":
                            comparisonOperator = SelectWhereComparisonOperatorEnum.Equal;
                            break;

                        case "<>":
                            comparisonOperator = SelectWhereComparisonOperatorEnum.NotEqual;
                            break;

                        case "!=":
                            comparisonOperator = SelectWhereComparisonOperatorEnum.NotEqual;
                            break;

                        case "<":
                            comparisonOperator = SelectWhereComparisonOperatorEnum.LessThan;
                            break;

                        case "!<":
                            negate = true;
                            comparisonOperator = SelectWhereComparisonOperatorEnum.LessThan;
                            break;

                        case "<=":
                            comparisonOperator = SelectWhereComparisonOperatorEnum.LessEqualThan;
                            break;

                        case ">":
                            comparisonOperator = SelectWhereComparisonOperatorEnum.GreaterThan;
                            break;

                        case "!>":
                            negate = true;
                            comparisonOperator = SelectWhereComparisonOperatorEnum.GreaterThan;
                            break;

                        case ">=":
                            comparisonOperator = SelectWhereComparisonOperatorEnum.GreaterEqualThan;
                            break;

                        default:
                            throw new Exception($"Found unexpected comparison operator '{comparisonOperator}'");
                    }

                    nextWord = clauseWords[index];

                    var rightExpression = ParseWhereColumn(nextWord);

                    var clause = new SelectWhereComparison(leftExpression, comparisonOperator, rightExpression)
                    {
                        Negate = negate
                    };

                    currentGroup.Clauses.Add(clause);
                }

                if (nextWord == "like")
                {
                    nextWord = clauseWords[++index];

                    var rightExpression = ParseWhereColumn(nextWord);

                    if (++index < clauseWords.Count)
                    {
                        throw new Exception("Found unexpected tokens after 'LIKE' clause.");
                    }

                    var clause = new SelectWhereLike(leftExpression, rightExpression)
                    {
                        Negate = negate
                    };

                    currentGroup.Clauses.Add(clause);
                }

                if (nextWord == "is")
                {
                    if (negate)
                    {
                        throw new Exception("'NOT' must come after 'IS'.");
                    }

                    nextWord = clauseWords[++index];

                    if (nextWord.NotIn("not", "null"))
                    {
                        throw new Exception("Expecting 'NOT' or 'NULL' after 'IS'.");
                    }

                    if (nextWord == "not")
                    {
                        negate = true;

                        nextWord = clauseWords[++index];

                        if (nextWord.NotIn("null"))
                        {
                            throw new Exception("Expecting 'NULL' after 'IS NOT'.");
                        }
                    }

                    if (nextWord != "null")
                    {
                        throw new Exception("Expecting 'NULL' after 'IS'.");
                    }

                    if (++index < clauseWords.Count)
                    {
                        throw new Exception("Found unexpected tokens after 'IS' clause.");
                    }

                    var clause = new SelectWhereNull(leftExpression)
                    {
                        Negate = negate
                    };

                    currentGroup.Clauses.Add(clause);
                }
            }

            clauseWords.Clear();
        }

        public static SelectWhereColumnBase ParseWhereColumn(string name)
        {
            SelectWhereColumnBase columnBase = null;
            int number;

            if (name.IsQuoted() || int.TryParse(name, out number))
            {
                columnBase = new SelectWhereRawColumn(name);
            }
            else if (char.IsLetter(name[0]))
            {
                columnBase = new SelectWhereFieldColumn(name);
            }

            return columnBase;
        }

        public static SelectWhereComparisonOperatorEnum ParseWhereClauseOperator(string text)
        {
            if (text == "=") return SelectWhereComparisonOperatorEnum.Equal;
            if (text == "<>") return SelectWhereComparisonOperatorEnum.NotEqual;

            if (text == ">") return SelectWhereComparisonOperatorEnum.GreaterThan;
            if (text == ">=") return SelectWhereComparisonOperatorEnum.GreaterEqualThan;

            if (text == "<") return SelectWhereComparisonOperatorEnum.LessThan;
            if (text == "<=") return SelectWhereComparisonOperatorEnum.LessEqualThan;

            return SelectWhereComparisonOperatorEnum.None;
        }
    }
}
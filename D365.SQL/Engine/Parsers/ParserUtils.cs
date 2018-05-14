namespace D365.SQL.Engine.Parsers
{
    using System.Collections.Generic;
    using System.Text;
    using DML.Select.Columns;
    using DML.Select.From;

    internal static class ParserUtils
    {
        public static IEnumerable<string> GetWords(string sql)
        {
            var sb = new StringBuilder();

            sql = sql.Trim();

            var inQuotes = false;

            for (int i = 0; i < sql.Length; i++)
            {
                var prevC = i > 0 ? sql[i - 1] : '\0';
                var c = sql[i];
                var nextC = i < sql.Length - 2 ? sql[i + 1] : '\0';

                if (char.IsLetterOrDigit(c) || c == '\'' || inQuotes || (c == '.' && i > 0 && i < sql.Length - 1 && char.IsLetter(sql[i - 1]) && char.IsLetter(sql[i + 1])))
                {
                    if (c == '\'')
                    {
                        inQuotes = !inQuotes;

                        if (inQuotes)
                        {
                            if (sql[i - 1] != ' ')
                            {
                                yield return sb.ToString();
                                sb.Clear();
                            }
                        }
                        else
                        {
                            sb.Append(c);
                            yield return sb.ToString();
                            sb.Clear();
                            continue;
                        }
                    }

                    sb.Append(c);
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        if (sb.ToString() == "order")
                        {
                            if (sql.Length >= i + 3)
                            {
                                var str = sql.Substring(i + 1, 2);
                                if (str == "by")
                                {
                                    sb.Append(" by");
                                    i += 3;
                                }
                            }
                        }

                        if (sb.ToString() == "group")
                        {
                            if (sql.Length >= i + 3)
                            {
                                var str = sql.Substring(i + 1, 2);
                                if (str == "by")
                                {
                                    sb.Append(" by");
                                    i += 3;
                                }
                            }
                        }

                        yield return sb.ToString();
                        sb.Clear();
                    }

                    if (c != ' ' && char.IsWhiteSpace(c) == false)
                        yield return c.ToString();
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

        public static List<List<string>> SplitTokens(List<string> words)
        {
            var columns = new List<List<string>>();
            List<string> list = null;
            var parenthesesCount = 0;

            for (int i = 0; i < words.Count; i++)
            {
                if (list == null)
                {
                    list = new List<string>();
                }

                if (words[i] == "(")
                {
                    parenthesesCount++;
                }

                if (words[i] == ")")
                {
                    parenthesesCount--;
                }

                if (parenthesesCount == 0 && words[i] == ",")
                {
                    columns.Add(list);
                    list = null;
                }
                else
                {
                    list.Add(words[i]);
                }
            }

            if (list != null) columns.Add(list);

            return columns;
        }

        public static ColumnParseResult ParseColumn(this string column)
        {
            var result = new ColumnParseResult();

            if (column.IsNotEmpty())
            {
                var parts = column.Split('.');

                result.Column = parts[parts.Length - 1];

                if (parts.Length > 2)
                {
                    result.Server = parts[parts.Length - 3];
                }

                if (parts.Length > 1)
                {
                    result.Alias = parts[parts.Length - 2];
                }
            }

            return result;
        }

        public static FromParseResult ParseFrom(this string from)
        {
            var result = new FromParseResult();

            if (from.IsNotEmpty())
            {
                var parts = from.Split('.');

                result.Name = parts[parts.Length - 1];

                if (parts.Length > 1)
                {
                    result.Server = parts[parts.Length - 2];
                }
            }

            return result;
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotEmpty(this string str)
        {
            return !str.IsEmpty();
        }
    }
}


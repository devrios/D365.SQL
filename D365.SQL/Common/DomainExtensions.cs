namespace D365.SQL.Common
{
    internal static class DomainExtensions
    {
        public static string CleanRaw(this string text)
        {
            if (text.IsEmpty())
            {
                return text;
            }

            if (text.StartsWith("'") && text.EndsWith("'"))
            {
                return text.Substring(1, text.Length - 2);
            }

            return text;
        }

        public static bool IsQuoted(this string text)
        {
            return text.StartsWith("'") && text.EndsWith("'");
        }
    }
}
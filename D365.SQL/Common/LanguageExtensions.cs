namespace D365.SQL.Common
{
    internal static class LanguageExtensions
    {
        public static bool IsEmpty(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        public static bool IsNotEmpty(this string source)
        {
            return !IsEmpty(source);
        }

        public static bool In<T>(this T source, params T[] values)
        {
            foreach (var value in values)
            {
                if (object.Equals(source, value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool NotIn<T>(this T source, params T[] values)
        {
            return !In(source, values);
        }
    }
}
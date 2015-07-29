namespace Orienteer
{
    public static class StringExtensions
    {
        public static string ToRoute(this string @this)
        {
            return @this[0] == '/' ? @this.Substring(1) : @this;
        }

        public static string WithUrlEncoding(this string @this)
        {
            return System.Net.WebUtility.UrlEncode(@this);
        }

        public static string WithoutUrlEncoding(this string @this)
        {
            return System.Net.WebUtility.UrlDecode(@this);
        }
    }
}
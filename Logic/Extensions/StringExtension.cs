using System;

namespace Logic.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Contains ignore case
        /// </summary>
        /// <param name="str"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string str, string token) => str.Contains(token, StringComparison.InvariantCultureIgnoreCase);
    }
}
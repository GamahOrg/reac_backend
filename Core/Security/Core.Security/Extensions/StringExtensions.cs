

namespace Core.Security.Extensions
{
    public static class StringExtensions
    {
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (postFixes == null)
            {
                return str;
            }

            foreach (string text in postFixes)
            {
                if (str.EndsWith(text))
                {
                    return str.Left(str.Length - text.Length);
                }
            }

            return str;
        }


        //
        // Summary:
        //     Gets a substring of a string from beginning of the string.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        //
        //   T:System.ArgumentException:
        //     Thrown if len is bigger that string's length
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }
    }

}
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SelfDrvn.Feeds.Share.Tools
{
    public static class StringTools
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars.ToLower(), length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string StripHTML(string input) => Regex.Replace(input, "<.*?>", String.Empty);

        public static string CleanNameTag(string content, string userIdFormat, bool strong)
        {
            string pattern = @"@\{([^:]+):([a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\}";
            if (string.IsNullOrEmpty(userIdFormat) ? false : string.Equals(userIdFormat, "userid"))
            {
                pattern = @"@\{([^:]+):(([{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?)|([a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?))\}";
                return Regex.Replace(content, pattern, delegate (Match match)
                {
                    string v = match.Value.Split(':', StringSplitOptions.RemoveEmptyEntries)[0];
                    v = v.Replace("@{", "<strong>");
                    v = v + "</strong>";
                    return v;
                }, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            return Regex.Replace(content, pattern, delegate (Match match)
            {
                string v = match.Value.Split(':', StringSplitOptions.RemoveEmptyEntries)[0];
                v = v.Replace("@{", "<strong>");
                v = v + "</strong>";
                return v;
            }, RegexOptions.IgnoreCase);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class URLHelper
    {
        public static string UrlSlug(string val)
        {
            string result = val;
            if (result.Contains("."))
            {
                result = result.Replace(".", "dot");
            }
            if (result.Contains(" "))
            {
                result = result.Replace(" ", "_");
            }
            if (result.Contains("@"))
            {
                result = result.Replace("@", "et");
            }
            if (result.Contains("#"))
            {
                result = result.Replace("#", "hash");
            }

            return result;
        }

        public static string UrlSlugWithId(string val, string id)
        {
            return UrlSlug(val) + "_" + id + "_";
        }

        public static string ToFriendlyUrl(string urlToEncode)
        {
            urlToEncode = (urlToEncode ?? "").Trim().ToLower();

            StringBuilder url = new StringBuilder();

            foreach (char ch in urlToEncode)
            {
                switch (ch)
                {
                    case ' ':
                        url.Append('-');
                        break;
                    case '&':
                        url.Append("and");
                        break;
                    case '\'':
                        break;
                    case '/':
                        break;
                    default:
                        if ((ch >= '0' && ch <= '9') ||
                            (ch >= 'a' && ch <= 'z'))
                        {
                            url.Append(ch);
                        }
                        else
                        {
                            url.Append('-');
                        }
                        break;
                }
            }

            return url.ToString();
        }

        public static string ToFriendlyUrl(string urlToEncode, string id)
        {
            if (id.Contains("__"))
            {

                throw new ArgumentException("Id can't containt __ ");
            }
            string result = string.Format("{0}__{1}__", ToFriendlyUrl(urlToEncode), id);
            return result;
        }

        public static string GetIdFromString(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("UrlIsNull");
            }
            if (!url.EndsWith("__"))
            {
                throw new InvalidOperationException("This is not URL with Id");
            }
            var startOfId = url.IndexOf("__");
            var endOfId = url.IndexOf("__", startOfId + 2);

            if (endOfId < startOfId || endOfId == startOfId)
            {
                throw new InvalidOperationException("This is not URL with Id");
            }
            var extractedId = url.Substring(startOfId + 2, endOfId - startOfId - 2);

            return extractedId;
        }

    }
}

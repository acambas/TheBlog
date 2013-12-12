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

            return result;
        }

        public static string UrlSlugWithId(string val, string id)
        {
            return UrlSlug(val) + "_" + id + "_";
        }

    }
}

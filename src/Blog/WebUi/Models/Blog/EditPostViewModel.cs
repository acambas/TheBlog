using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebUi.Models.Blog
{
    public class EditPostViewModel : CreatePostViewModel
    {

        public string getTagsValue()
        {
            StringBuilder b = new StringBuilder();
            var tags = Tags.ToArray();
            for (int i = 0; i < tags.Length; i++)
            {
                b.Append(tags[i]);
                if (tags.Length != i + 1)
                {
                    b.Append(",");
                }
            }
            return b.ToString();
        }
    }
}
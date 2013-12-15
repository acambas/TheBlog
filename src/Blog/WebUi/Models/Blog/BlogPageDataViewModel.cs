using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUi.Models.Blog
{
    public class BlogPageDataViewModel
    {
        public BlogPageDataViewModel()
        {
            TagCountList = new List<TagCountViewModel>();
            RecentLinkList = new List<PostLinkViewModel>();
        }

        public IEnumerable<TagCountViewModel> TagCountList { get; set; }
        public IEnumerable<PostLinkViewModel> RecentLinkList { get; set; }
    }
}
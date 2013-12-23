using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUi.App_Start;

namespace WebUi.Models.Blog
{
    public class CreatePostViewModel
    {
        protected IEnumerable<string> allTags = new List<string>();

        public CreatePostViewModel()
        {
            allTags = new List<string>();
            
            Tags = new List<string>();
        }

        public virtual string UrlSlug { get; set; }

        [Required(ErrorMessage = "Title: Field is required")]
        [StringLength(500, ErrorMessage = "Title: Length should not exceed 500 characters")]
        public virtual string Title { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "ShortDescription: Field is required")]
        public virtual string ShortDescription { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Description: Field is required")]
        public virtual string Description { get; set; }

        public virtual IEnumerable<string> Tags { get; set; }


        public virtual IEnumerable<SelectListItem> SelectItemsForTags()
        {
            return allTags
                .Select(m => new SelectListItem { Text = m, Value = m });
        }

        public virtual void FillAllTags(IEnumerable<TagViewModel> tags)
        {
            allTags = tags.Select(m => m.Name);
        }

        public virtual string[] ArrayOfAllTags()
        {
            return allTags.ToArray();
        }

    }
}
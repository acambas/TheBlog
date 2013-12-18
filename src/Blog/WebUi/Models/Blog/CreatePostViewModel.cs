using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUi.Models.Blog
{
    public class CreatePostViewModel
    {
        public CreatePostViewModel()
        {
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
    }
}
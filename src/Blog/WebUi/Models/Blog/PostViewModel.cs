using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebUi.Models.Blog
{
    public class PostViewModel
    {
        public PostViewModel()
        {
            Tags = new List<string>();
        }

        public virtual string UrlSlug { get; set; }

        [Required(ErrorMessage = "Title: Field is required")]
        [StringLength(500, ErrorMessage = "Title: Length should not exceed 500 characters")]
        public virtual string Title { get; set; }

        [Required(ErrorMessage = "ShortDescription: Field is required")]
        public virtual string ShortDescription { get; set; }

        [Required(ErrorMessage = "Description: Field is required")]
        public virtual string Description { get; set; }

        public virtual IEnumerable<string> Tags { get; set; }
    }
}
using Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Post
{
    public class Post : EntityBase<string>
    {
        public Post()
        {
            LastEdit = DateTime.Now;
            Active = true;
            Published = true;
        }

        [Required(ErrorMessage = "Title: Field is required")]
        [StringLength(500, ErrorMessage = "Title: Length should not exceed 500 characters")]
        public virtual string Title
        { get; set; }

        [Required(ErrorMessage = "ShortDescription: Field is required")]
        public virtual string ShortDescription
        { get; set; }

        [Required(ErrorMessage = "Description: Field is required")]
        public virtual string Description
        { get; set; }

        [Required(ErrorMessage = "Meta: Field is required")]
        [StringLength(1000, ErrorMessage = "Meta: UrlSlug should not exceed 50 characters")]
        public virtual string UrlSlug
        { get; set; }

        public virtual bool Published
        { get; set; }

        [Required(ErrorMessage = "PostedOn: Field is required")]
        public virtual DateTime PostedOn
        { get; set; }

        public virtual DateTime? Modified
        { get; set; }

        public virtual IEnumerable<Tag.Tag> Tags
        { get; set; }

        public override bool Validate()
        {
            return true;
        }
    }
}
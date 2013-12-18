using System.ComponentModel.DataAnnotations;

namespace Domain.Tag
{
    public class Tag
    {
        //[Required(ErrorMessage = "Name: Field is required")]
        [StringLength(500, ErrorMessage = "Name: Length should not exceed 500 characters")]
        public virtual string Name
        { get; set; }

        //[Required(ErrorMessage = "UrlSlug: Field is required")]
        [StringLength(500, ErrorMessage = "UrlSlug: Length should not exceed 500 characters")]
        public virtual string UrlSlug
        { get; set; }
    }
}
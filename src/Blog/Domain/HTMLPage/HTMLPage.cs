using Infrastructure.Domain;
using System.ComponentModel.DataAnnotations;

namespace Domain.HTMLPage
{
    public class HTMLPage : EntityBase<string>
    {
        [StringLength(500, ErrorMessage = "Name: Length should not exceed 500 characters")]
        public virtual string Name
        { get; set; }

        public virtual string HTML
        { get; set; }

        public override bool Validate()
        {
            return true;
        }
    }
}
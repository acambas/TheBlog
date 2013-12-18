using Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

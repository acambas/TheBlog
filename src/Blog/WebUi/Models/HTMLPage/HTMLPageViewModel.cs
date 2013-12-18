using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
namespace WebUi.Models.HTMLPage
{
    public class HTMLPageViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Name: Length should not exceed 500 characters")]
        public virtual string Name
        { get; set; }

        [AllowHtml]
        public virtual string HTML
        { get; set; }
    }
}
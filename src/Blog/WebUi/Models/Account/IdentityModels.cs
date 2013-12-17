using AspNet.Identity.RavenDB.Entities;
using Infrastructure.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebUi.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : RavenUser, IEntityBase<string>
    {

        public ApplicationUser()
        {
            LastEdit = DateTime.Now;
            Active = true;
        }

        public string Email { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public bool Validate()
        {
            return true;
        }
        public DateTime LastEdit { get; set; }
    }


}
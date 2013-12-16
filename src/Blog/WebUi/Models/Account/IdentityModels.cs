using AspNet.Identity.RavenDB.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebUi.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : RavenUser
    {
        public string Email { get; set; }

        public string Name { get; set; }
    }


}
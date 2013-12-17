using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using WebUi.App_Start;
using System.Linq;
using System;
using System.Security.Claims;
using AspNet.Identity.RavenDB.Entities;
namespace WebUi.Models
{

    public class ApplicationUserViewModel
    {
        string[] roles;
        string selectedRole;
        public ApplicationUserViewModel()
        {
            roles = AppRoles.AppRoleList.ToArray();
        }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "User real name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "User email address")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Id { get; set; }

        public void SelectedRole(string role)
        {
            if (!roles.Contains(role))
                throw new ArgumentException("role doesn't exist");

            selectedRole = role;
                
        }

        public static string GetRoleFromClaim(RavenUserClaim claim)
        {
            if (claim == null || claim.ClaimType==null || claim.ClaimValue == null || claim.ClaimType != ClaimTypes.Role)
                throw new ArgumentException();
            return claim.ClaimValue;
        }

        public IEnumerable<System.Web.Mvc.SelectListItem> RoleSelectListItems()
        {
            return roles.Select(m => new System.Web.Mvc.SelectListItem { Text = m, Value = m, Selected = (m == selectedRole) });
        }
    }

    public class RegisterViewModel
    {
        string[] roles;
        public RegisterViewModel()
        {
            roles = AppRoles.AppRoleList;
        }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "User real name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "User email address")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        public string Role { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> RoleSelectListItems()
        {
            return roles.Select(m => new System.Web.Mvc.SelectListItem { Text = m, Value = m });
        }
    }


    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


}

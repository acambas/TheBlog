using System.Linq;
using System.Security.Claims;

namespace WebUi.App_Start
{
    public static class AppAuthorizationType
    {
        public const string RoleAuth = "RoleAuth";
    }

    public sealed class AppRoles
    {
        public const string Admin = "Admin";
        public const string Edit = "Edit";
        public const string Read = "Read";
        public static readonly string[] AppRoleList = new string[] { Admin, Edit, Read };
    }

    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            switch (context.Action.First().Value)
            {
                case AppAuthorizationType.RoleAuth:
                    {
                        if (!HandleRoleAuth(context))
                            return false;
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        private bool HandleRoleAuth(AuthorizationContext context)
        {
            var roleClaims = context.Resource;

            //If no role is specified user can perform the action
            if (roleClaims == null || roleClaims.Count == 0)
            {
                return true;
            }

            //If user has admin claim he can do all actions
            if (roleClaims.Any(m => m.Value == AppRoles.Admin.ToString() ||
                m.Value == AppRoles.Edit.ToString() ||
                m.Value == AppRoles.Read.ToString()
                ))
            {
                var userClaim = context.Principal.Claims
                    .Any(m => m.Type == ClaimTypes.Role && m.Value == AppRoles.Admin);
                if (userClaim)
                    return true;
            }

            //If user has edit claim he can also do read action
            if (roleClaims.Any(m => m.Value == AppRoles.Edit.ToString() ||
                m.Value == AppRoles.Read.ToString()
                ))
            {
                var userClaim = context.Principal.Claims
                    .Any(m => m.Type == ClaimTypes.Role && m.Value == AppRoles.Edit);
                if (userClaim)
                    return true;
            }

            //If user has read claim he can only do read actions
            if (roleClaims.Any(m => m.Value == AppRoles.Read.ToString()))
            {
                var userClaim = context.Principal.Claims
                    .Any(m => m.Type == ClaimTypes.Role && m.Value == AppRoles.Read);
                if (userClaim)
                    return true;
            }

            return false;
        }
    }
}
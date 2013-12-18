﻿using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Infrastructure.Config._Settings;
using Infrastructure.Mapping;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Authorization.Mvc;
using WebUi.App_Start;
using WebUi.Controllers;
using WebUi.Models;

namespace WebUi.Areas.Admin.Controllers
{
    [Authorize]
    public class AccountController : RavenController
    {
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public AccountController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings)
            : base(logger, mapper, appSettings)
        {
            if (RavenSession == null)
            {
                RavenSession = MvcApplication.Store.OpenAsyncSession();
            }
            UserManager = new UserManager<ApplicationUser>
                (new RavenUserStore<ApplicationUser>(RavenSession));
        }

        [ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        public async Task<ActionResult> Index()
        {
            IEnumerable<ApplicationUserViewModel> viewModel;
            var data = await RavenSession.Query<ApplicationUser>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(m => m.Active == true)
                .ToListAsync();

            viewModel = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(data);
            return View(viewModel);
        }

        [ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUserViewModel viewModel;
            var data = await RavenSession.Query<ApplicationUser>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstAsync(m => m.UserName == id);
            viewModel = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(data);

            var claimRole = data.Claims.FirstOrDefault(m => m.ClaimType == ClaimTypes.Role);
            if (claimRole != null)
                viewModel.SelectedRole(claimRole.ClaimValue);

            return View("EditUser", viewModel);
        }

        [HttpPost]
        [ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        public async Task<ActionResult> Edit(ApplicationUserViewModel viewModel)
        {
            if (ModelState.IsValid && AppRoles.AppRoleList.Contains(viewModel.Role))
            {
                var data = await RavenSession.Query<ApplicationUser>()
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                    .FirstAsync(m => m.UserName == viewModel.UserName);

                //Check for id match
                if (data.Id != viewModel.Id || data.UserName != viewModel.UserName)
                    throw new ArgumentException("User Id does not match");
                data.Active = true;
                data = Mapper.Map<ApplicationUserViewModel, ApplicationUser>(viewModel, data);

                //Change role
                var claimRole = data.Claims.FirstOrDefault(m => m.ClaimType == ClaimTypes.Role);
                if (claimRole != null)
                    claimRole.ClaimValue = viewModel.Role;
                else
                {
                    if (data.Claims == null)
                        data.Claims = new List<RavenUserClaim>();

                    data.Claims.Add(new RavenUserClaim(new Claim(ClaimTypes.Role, viewModel.Role)));
                }
                await RavenSession.StoreAsync(data);
                await SaveAsync();
                return RedirectToAction("Index");
            }

            return View("EditUser", viewModel);
        }

        [HttpPost]
        [ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUserViewModel viewModel;
            var data = await RavenSession.Query<ApplicationUser>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstAsync(m => m.UserName == id);
            viewModel = Mapper.Map<ApplicationUser, ApplicationUserViewModel>(data);
            return View("EditUser", viewModel);
        }

        [AllowAnonymous]
        [ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return RedirectToAction("Login", "Login", new { Area = "" });
        }

        [ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ClaimsAuthorize(AppAuthorizationType.RoleAuth, AppRoles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid && AppRoles.AppRoleList.Contains(model.Role))
            {
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, Name = model.Name };
                user = Mapper.Map<RegisterViewModel, ApplicationUser>(model);
                user.Claims = new List<RavenUserClaim> { new RavenUserClaim(new Claim(ClaimTypes.Role, model.Role)) };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Account", new { Area = "Admin" });
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }

            public string RedirectUri { get; set; }

            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}
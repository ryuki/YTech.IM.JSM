using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using SharpArch.Core;
using SharpArch.Web.NHibernate;
using YTech.IM.JSM.Enums;
using YTech.IM.JSM.Web.Controllers.ViewModel;

using System.Net.Mail;
using System.Web.Security;
using MvcMembership;
using MvcMembership.Settings;
using YTech.IM.JSM.Web.Controllers.ViewModel.UserAdministration;
using System.Security.Principal;

namespace YTech.IM.JSM.Web.Controllers.Utility
{
    //[Authorize(Roles = "Administrator")]
    public class UserAdministrationController : Controller
    {
        private const int PageSize = 10;
        private const string ResetPasswordBody = "Your new password is: ";
        private const string ResetPasswordFromAddress = "from@domain.com";
        private const string ResetPasswordSubject = "Your New Password";
        private readonly IRolesService _rolesService;
        private readonly ISmtpClient _smtpClient;
        private readonly IMembershipSettings _membershipSettings;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;

        public UserAdministrationController()
            : this(
                new AspNetMembershipProviderSettingsWrapper(Membership.Provider),
                new AspNetMembershipProviderWrapper(Membership.Provider),
                new AspNetMembershipProviderWrapper(Membership.Provider),
                new AspNetRoleProviderWrapper(Roles.Provider),
                new SmtpClientProxy(new SmtpClient()),
            null,
            null)
        {
        }


        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        public UserAdministrationController(
            IMembershipSettings membershipSettings,
            IUserService userService,
            IPasswordService passwordService,
            IRolesService rolesService,
            ISmtpClient smtpClient, IMembershipService service, IFormsAuthentication formsAuth)
        {
            _membershipSettings = membershipSettings;
            _userService = userService;
            _passwordService = passwordService;
            _rolesService = rolesService;
            _smtpClient = smtpClient;
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }

        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public ViewResult Index(int? index)
        {
            return View(new IndexViewModel
                            {
                                Users = _userService.FindAll(index ?? 0, PageSize),
                                Roles = _rolesService.FindAll()
                            });
        }

        public ViewResult ListUsers()
        {
            return View();
        }


        [Transaction]
        public virtual ActionResult List(string sidx, string sord, int page, int rows)
        {
            var membershipUsers = _userService.FindAll(page - 1, rows);
            int totalRecords = membershipUsers.Count;
            int pageSize = rows;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from u in membershipUsers
                    select new
                    {
                        i = u.ProviderUserKey.ToString(),
                        cell = new string[] {
                            string.Empty,
                            u.ProviderUserKey.ToString(), 
                            u.UserName, 
                            string.Empty,
                            string.Empty, 
                            u.Comment, 
                            GetOfflineSince(u.LastActivityDate)
                        }
                    }).ToArray()
            };


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        private static string GetOfflineSince(DateTime lastActivityDate)
        {
            string offlineSinceTime;
            var offlineSince = (DateTime.Now - lastActivityDate);
            if (offlineSince.TotalSeconds <= 60)
                offlineSinceTime = "1 menit";
            else if (offlineSince.TotalMinutes < 60)
                offlineSinceTime = Math.Floor(offlineSince.TotalMinutes) + " menit";
            else if (offlineSince.TotalMinutes < 120)
                offlineSinceTime = "1 jam";
            else if (offlineSince.TotalHours < 24)
                offlineSinceTime = Math.Floor(offlineSince.TotalHours) + " jam";
            else if (offlineSince.TotalHours < 48)
                offlineSinceTime = "1 hari";
            else
                offlineSinceTime = Math.Floor(offlineSince.TotalDays) + " hari";
            return string.Format("{0} yang lalu", offlineSinceTime);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult CreateRole(string id)
        {
            _rolesService.Create(id);
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult DeleteRole(string id)
        {
            _rolesService.Delete(id);
            return RedirectToAction("Index");
        }

        public ViewResult Role(string id)
        {
            return View(new RoleViewModel
                            {
                                Role = id,
                                Users = _rolesService.FindUserNamesByRole(id).Select(username => _userService.Get(username))
                            });
        }

        public ViewResult Details(Guid id)
        {
            var user = _userService.Get(id);
            var userRoles = _rolesService.FindByUser(user);
            return View(new DetailsViewModel
                            {
                                CanResetPassword = _membershipSettings.Password.ResetOrRetrieval.CanReset,
                                RequirePasswordQuestionAnswerToResetPassword = _membershipSettings.Password.ResetOrRetrieval.RequiresQuestionAndAnswer,
                                DisplayName = user.UserName,
                                User = user,
                                Roles = _rolesService.FindAll().ToDictionary(role => role, role => userRoles.Contains(role)),
                                Status = user.IsOnline
                                            ? DetailsViewModel.StatusEnum.Online
                                            : !user.IsApproved
                                                ? DetailsViewModel.StatusEnum.Unapproved
                                                : user.IsLockedOut
                                                    ? DetailsViewModel.StatusEnum.LockedOut
                                                    : DetailsViewModel.StatusEnum.Offline
                            });
        }

        public ViewResult Password(Guid id)
        {
            var user = _userService.Get(id);
            var userRoles = _rolesService.FindByUser(user);
            return View(new DetailsViewModel
            {
                CanResetPassword = _membershipSettings.Password.ResetOrRetrieval.CanReset,
                RequirePasswordQuestionAnswerToResetPassword = _membershipSettings.Password.ResetOrRetrieval.RequiresQuestionAndAnswer,
                DisplayName = user.UserName,
                User = user,
                Roles = _rolesService.FindAll().ToDictionary(role => role, role => userRoles.Contains(role)),
                Status = user.IsOnline
                            ? DetailsViewModel.StatusEnum.Online
                            : !user.IsApproved
                                ? DetailsViewModel.StatusEnum.Unapproved
                                : user.IsLockedOut
                                    ? DetailsViewModel.StatusEnum.LockedOut
                                    : DetailsViewModel.StatusEnum.Offline
            });
        }

        public ViewResult UsersRoles(Guid id)
        {
            var user = _userService.Get(id);
            var userRoles = _rolesService.FindByUser(user);
            return View(new DetailsViewModel
            {
                CanResetPassword = _membershipSettings.Password.ResetOrRetrieval.CanReset,
                RequirePasswordQuestionAnswerToResetPassword = _membershipSettings.Password.ResetOrRetrieval.RequiresQuestionAndAnswer,
                DisplayName = user.UserName,
                User = user,
                Roles = _rolesService.FindAll().ToDictionary(role => role, role => userRoles.Contains(role)),
                Status = user.IsOnline
                            ? DetailsViewModel.StatusEnum.Online
                            : !user.IsApproved
                                ? DetailsViewModel.StatusEnum.Unapproved
                                : user.IsLockedOut
                                    ? DetailsViewModel.StatusEnum.LockedOut
                                    : DetailsViewModel.StatusEnum.Offline
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult Details(Guid id, string email, string comments)
        {
            var user = _userService.Get(id);
            user.Email = email;
            user.Comment = comments;
            _userService.Update(user);
            return RedirectToAction("Details", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult DeleteUser(Guid id)
        {
            _userService.Delete(_userService.Get(id));
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(FormCollection formCollection)
        {
            try
            {
                string userName = formCollection["id"];
                _userService.Delete(_userService.Get(userName));
                return Content("Success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
                throw;
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult ChangeApproval(Guid id, bool isApproved)
        {
            var user = _userService.Get(id);
            user.IsApproved = isApproved;
            _userService.Update(user);
            return RedirectToAction("Details", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult Unlock(Guid id)
        {
            _passwordService.Unlock(_userService.Get(id));
            return RedirectToAction("Details", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult ResetPassword(Guid id)
        {
            var user = _userService.Get(id);
            var newPassword = _passwordService.ResetPassword(user);

            var body = ResetPasswordBody + newPassword;
            _smtpClient.Send(new MailMessage(ResetPasswordFromAddress, user.Email, ResetPasswordSubject, body));

            return RedirectToAction("Password", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult ResetPasswordWithAnswer(Guid id, string answer)
        {
            var user = _userService.Get(id);
            var newPassword = _passwordService.ResetPassword(user, answer);

            var body = ResetPasswordBody + newPassword;
            _smtpClient.Send(new MailMessage(ResetPasswordFromAddress, user.Email, ResetPasswordSubject, body));

            return RedirectToAction("Password", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult SetPassword(Guid id, string password)
        {
            var user = _userService.Get(id);
            _passwordService.ChangePassword(user, password);

            var body = ResetPasswordBody + password;
            _smtpClient.Send(new MailMessage(ResetPasswordFromAddress, user.Email, ResetPasswordSubject, body));

            return RedirectToAction("Password", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult AddToRole(Guid id, string role)
        {
            _rolesService.AddToRole(_userService.Get(id), role);
            return RedirectToAction("UsersRoles", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult RemoveFromRole(Guid id, string role)
        {
            _rolesService.RemoveFromRole(_userService.Get(id), role);
            return RedirectToAction("UsersRoles", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(string userName, string email, string password, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword))
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(userName, password, email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuth.SignIn(userName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Insert(FormCollection formCollection)
        {
            string userName = formCollection["UserName"];
            string email = "yahu@yahu.com";
            string password = formCollection["Password"];
            string confirmPassword = formCollection["PasswordConfirm"];

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword))
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(userName, password, email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsAuth.SignIn(userName, false /* createPersistentCookie */);
                    return Content("success");
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                    return Content(ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }


}
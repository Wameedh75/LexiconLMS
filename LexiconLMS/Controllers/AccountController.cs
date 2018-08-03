using LexiconLMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LexiconLMS.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEnumerable<SelectListItem> avilableCourses;
        private IEnumerable<SelectListItem> availAbleRoles;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        //

        [Authorize(Roles = "teacher")]
        public ActionResult Teachers()
        {
            var userdb = ApplicationDbContext.Create();

            var roleId = userdb.Roles.FirstOrDefault(r => r.Name == "teacher")?.Id;
            var teachers = userdb.Users.Where(u => u.Roles.Select(y => y.RoleId).Contains(roleId)).ToList();

            //var x2 = userdb.Roles.Where(r => r.Name == "teacher").FirstOrDefault().Id;
            //var x3 = userdb.Users.Where(r => r.Roles.Select(jr => jr.RoleId).FirstOrDefault()==x2).ToList();

            return View(teachers);
        }

        [Authorize(Roles = "teacher")]
        public ActionResult Students()
        {
            var userdb = ApplicationDbContext.Create();

            var roleId = userdb.Roles.FirstOrDefault(r => r.Name == "student")?.Id;
            var students = userdb.Users.Where(u => u.Roles.Select(y => y.RoleId).Contains(roleId)).ToList();

            return View(students);
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            //here we check if the user cinfirmed his email , if not we don't let him login and send him confirmation email again 
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null && !await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                #region //create message body
                StringBuilder message = new StringBuilder();
                // generating the message
                message.Append("Mr " + user.FullName + " ! You Need to Confirm your Email before you can login");
                message.Append(".<br/>");
                message.AppendLine(" Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a><br/>");
                #endregion
                await UserManager.SendEmailAsync(user.Id, "Confirm your account So you can login", message.ToString());
                ModelState.AddModelError("", "You need to confirm your account before login , w've sent you a new confirmation email , Check your Email To see it");
                return View(model);
            }
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        // GET: /Account/RegisterStudent
        [Authorize(Roles = "teacher")]
        public ActionResult RegisterStudent()
        {
            //create instance of the database &role store & role manager
            //so we can show the roles in the dropdown list t create the user
            var userdb = ApplicationDbContext.Create();

            //Show courses list
            var courseList = userdb.Courses.Select(c => new { c.Name, c.Id, c.EndDate }).Where(sa => sa.EndDate > DateTime.Today);
            List<SelectListItem> courseListWithDefault = courseList.Select(course => new SelectListItem { Text = course.Name, Value = course.Id.ToString() }).ToList();
            var courseTip = new SelectListItem
            {
                Value = null,
                Text = "--- Select Course---"
            };
            courseListWithDefault.Insert(0, courseTip);

            //create a template of the user registert view model and set its properties{lists} our we created
            var template = new RegisterStudentViewModel() { Courses = courseListWithDefault };

            return View(template);
        }

        // GET: /Account/RegisterTeacher
        [Authorize(Roles = "teacher")]
        public ActionResult RegisterTeacher()
        {
            return View();
        }

        #region
        //
        // GET: /Account/Register
        //[Authorize(Roles = "teacher")]
        //public ActionResult Register([Bind(Include= "rolestring")]string rolestring)
        //{
        //    //create instance of the database & role store & role manager 
        //    //so we can show the roles in the dropdown list t create the user
        //    var userdb = ApplicationDbContext.Create();
        //    var roleStore = new RoleStore<IdentityRole>(userdb);
        //    var roleMngr = new RoleManager<IdentityRole>(roleStore);

        //    //bring the roles from the database and create a list of it 
        //    //add default first item to the list "empty role to enhance the user experince"
        //    var rolesList = roleMngr.Roles.Select(r => new { r.Name, r.Id });
        //    List<SelectListItem> roleListWithDefault = rolesList.Select(role => new SelectListItem { Value = role.Id, Text = role.Name }).ToList();
        //    var roletip = new SelectListItem()
        //    {
        //        Value = null,
        //        Text = "--- Select Role ---"
        //    };
        //    roleListWithDefault.Insert(0, roletip);

        //    //same as before but courses list
        //    var courseList = userdb.Courses.Select(c => new { c.Name, c.Id, c.EndDate }).Where(sa => sa.EndDate > DateTime.Today);
        //    List<SelectListItem> courseListWithDefault = courseList.Select(course => new SelectListItem { Text = course.Name, Value = course.Id.ToString() }).ToList();
        //    var courseTip = new SelectListItem {
        //        Value = null,
        //        Text = "--- Select Course---"
        //    };
        //    courseListWithDefault.Insert(0, courseTip);

        //    //to improve UX we set default value of role when creating user to the role match role name passed as parameter
        //    if (rolestring!=null && roleMngr.Roles.Select(r=>r.Name).Contains(rolestring))
        //    {
        //        rolestring = roleMngr.Roles.FirstOrDefault(r => r.Name == rolestring).Id;
        //    }
        //    //create a template of the user registert view model and set its properties{lists} our we created
        //    var template = new RegisterViewModel() { Roles = roleListWithDefault, Courses = courseListWithDefault,SelectedRole=rolestring};

        //    return View(template);
        //}

        //
        // POST: /Account/Register

        //[HttpPost]
        //[Authorize(Roles = "teacher")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    var userdb = ApplicationDbContext.Create();
        //    var roleStore = new RoleStore<IdentityRole>(userdb);
        //    var roleMngr = new RoleManager<IdentityRole>(roleStore);

        //    var teacherRole = roleMngr.Roles.Select(r => new { r.Id, r.Name }).Where(r => r.Name == "teacher").Select(r => r.Id).FirstOrDefault();
        //    var studentRole = roleMngr.Roles.Select(r => new { r.Id, r.Name }).Where(r => r.Name == "student").Select(r => r.Id).FirstOrDefault();
        //    int? nullablecourseId;
        //    if (model.SelectedCourse != null)
        //    {
        //        int courseId = 0;
        //        var validCourseId = int.TryParse(model.SelectedCourse, out int parametercourseId);
        //        if (validCourseId)
        //        {
        //            courseId = parametercourseId;
        //        }

        //        //when admin create a student user we check for validation of choosed course .
        //        if (model.SelectedRole == studentRole)
        //        {
        //            if ((courseId < 1) && model.SelectedRole == studentRole && ((string.IsNullOrWhiteSpace(model.SelectedCourse) || !userdb.Courses.Select(cid => cid.Id).Contains(courseId))))
        //            {
        //                ModelState.AddModelError(string.Empty, "You Choosed Not valid Course");
        //            }
        //        }
        //        nullablecourseId = courseId;
        //    }
        //    else
        //    {
        //        nullablecourseId = null;
        //    }

        //    //when admin create a teacher user we check for there is no choosed course.
        //    if (model.SelectedRole == teacherRole && !(string.IsNullOrEmpty(model.SelectedCourse)))
        //    {
        //        ModelState.AddModelError(string.Empty, "The Teacher can't have a course");
        //    }

        //    if (model.SelectedRole != teacherRole && model.SelectedRole != studentRole)
        //    {
        //        ModelState.AddModelError(string.Empty, "You choosed a non valid role");
        //    }
        //    //check if the user already registered in our system
        //    var usercheck = await UserManager.FindByEmailAsync(model.Email);
        //    if (usercheck != null)
        //    {
        //        ModelState.AddModelError("", "User Already registered");
        //    }
        //    if (ModelState.IsValid)
        //    {

        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, CourseId = nullablecourseId };
        //        //Auto generate password and set it to the account
        //        string genpas = PasswordGenerator();
        //        var result = await UserManager.CreateAsync(user, genpas);
        //        //put the user in the choosed role
        //        UserManager.AddToRole(user.Id, userdb.Roles.Where(r => r.Id == model.SelectedRole).Select(r => r.Name).FirstOrDefault());

        //        if (result.Succeeded)
        //        {
        //            // await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

        //            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            #region //create message body
        //            StringBuilder message = new StringBuilder();
        //            // generating the message
        //            message.Append("Welcome " + user.FullName + "! You registered in our System as (");
        //            message.Append(UserManager.GetRoles(user.Id).FirstOrDefault() + ") .<br/>");
        //            message.AppendLine("And Your password is (" + genpas + ") .<br/>");
        //            message.AppendLine("We suggest you to change your auto generated pssword for more security .<br/>");
        //            message.AppendLine(" Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a><br/>");
        //            #endregion
        //            await UserManager.SendEmailAsync(user.Id, "Confirm your account", message.ToString());

        //            return RedirectToAction("Index", "Courses");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form

        //    //if error happened we need to re create the roles and courses list
        //    #region
        //    //create instance of the database & role store & role manager 
        //    //so we can show the roles in the dropdown list to create the user

        //    //bring the roles from the database and create a list of it 
        //    //add default first item to the list "empty role to enhance the user experince"
        //    var rolesList = roleMngr.Roles.Select(r => new { r.Name, r.Id });
        //    List<SelectListItem> roleListWithDefault = rolesList.Select(role => new SelectListItem { Value = role.Id, Text = role.Name }).ToList();
        //    var roletip = new SelectListItem()
        //    {
        //        Value = null,
        //        Text = "--- Select Role ---"
        //    };
        //    roleListWithDefault.Insert(0, roletip);

        //    //same as before but courses list
        //    var courseList = userdb.Courses.Select(c => new { c.Name, c.Id, c.EndDate }).Where(sa => sa.EndDate > DateTime.Today);
        //    List<SelectListItem> courseListWithDefault = courseList.Select(course => new SelectListItem { Text = course.Name, Value = course.Id.ToString() }).ToList();
        //    var courseTip = new SelectListItem
        //    {
        //        Value = null,
        //        Text = "--- Select Course---"
        //    };
        //    courseListWithDefault.Insert(0, courseTip);

        //    model.Roles = roleListWithDefault;
        //    model.Courses = courseListWithDefault;
        //    #endregion
        //    //create a template of the user registert view model and set its properties{lists} our we created
        //    return View(model);
        //}
        #endregion

        [HttpPost]
        [Authorize(Roles = "teacher")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterStudent(RegisterStudentViewModel model)
        {
            var userdb = ApplicationDbContext.Create();
            if (string.IsNullOrWhiteSpace(model.SelectedCourse))
            {
                ModelState.AddModelError("Courses", "You Choosed Not valid Course");
            }
            int courseId = 0;
            if (model.SelectedCourse != null)
            {
                var validCourseId = int.TryParse(model.SelectedCourse, out int parametercourseId);
                if (validCourseId)
                {
                    courseId = parametercourseId;
                }
                else
                {
                    ModelState.AddModelError("Courses", "You Choosed Not valid Course");
                }

                //when admin create a student user we check for validation of choosed course .
                if (courseId < 1)
                {
                    ModelState.AddModelError("Courses", "You Choosed Not valid Course");
                }
                if (!userdb.Courses.Select(cid => cid.Id).Contains(courseId))
                {
                    ModelState.AddModelError("Courses", "You Choosed Not valid Course");
                }

            }


            //check if the user already registered in our system
            if (ModelState.IsValid)
            {
                var usercheck = await UserManager.FindByEmailAsync(model.Email);
                if (usercheck != null)
                {
                    ModelState.AddModelError("Email", "User Already registered");
                }
            }
            if (!await RoleManager.RoleExistsAsync("student"))
            {
                ModelState.AddModelError("", "Unkown Error , Student role deleted from the database maybe?");
            }
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, CourseId = courseId };
                //Auto generate password and set it to the account
                string genpas = PasswordGenerator();
                var result = await UserManager.CreateAsync(user, genpas);
                //put the user in the choosed role
                await UserManager.AddToRoleAsync(user.Id, "student");
                
                if (result.Succeeded)
                {
                    // await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    #region //create message body
                    StringBuilder message = new StringBuilder();
                    // generating the message
                    message.Append("Welcome " + user.FullName + "! You registered in our System as student");
                    message.AppendLine("And Your password is (" + genpas + ") .<br/>");
                    message.AppendLine("We suggest you to change your auto generated pssword for more security .<br/>");
                    message.AppendLine(" Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a><br/>");
                    #endregion
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", message.ToString());

                    return RedirectToAction("Index", "Courses");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form

            //if error happened we need to re create the courses list
            #region


            //same as before but courses list
            var courseList = userdb.Courses.Select(c => new { c.Name, c.Id, c.EndDate }).Where(sa => sa.EndDate > DateTime.Today);
            List<SelectListItem> courseListWithDefault = courseList.Select(course => new SelectListItem { Text = course.Name, Value = course.Id.ToString() }).ToList();
            var courseTip = new SelectListItem
            {
                Value = null,
                Text = "--- Select Course---"
            };
            courseListWithDefault.Insert(0, courseTip);
            model.Courses = courseListWithDefault;
            #endregion
            //create a template of the user registert view model and set its properties{lists} our we created
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "teacher")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterTeacher(RegisterTeacherViewModel model)
        {
            var userdb = ApplicationDbContext.Create();


            //check if the user already registered in our system
            if (ModelState.IsValid)
            {
                var usercheck = await UserManager.FindByEmailAsync(model.Email);
                if (usercheck != null)
                {
                    ModelState.AddModelError("Email", "User Already registered");
                }
            }
            if (!await RoleManager.RoleExistsAsync("teacher"))
            {
                ModelState.AddModelError("", "Unkown Error , Teacher role deleted from the database maybe?");
            }
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber };
                //Auto generate password and set it to the account
                string genpas = PasswordGenerator();
                var result = await UserManager.CreateAsync(user, genpas);
                //put the user in the choosed role
                await UserManager.AddToRoleAsync(user.Id, "teacher");

                if (result.Succeeded)
                {
                    // await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    #region //create message body
                    StringBuilder message = new StringBuilder();
                    // generating the message
                    message.Append("Welcome " + user.FullName + "! You registered in our System as Teacher");
                    message.AppendLine("And Your password is (" + genpas + ") .<br/>");
                    message.AppendLine("We suggest you to change your auto generated pssword for more security .<br/>");
                    message.AppendLine(" Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a><br/>");
                    #endregion
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", message.ToString());

                    return RedirectToAction("Index", "Courses");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form

            return View();
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string confirmCode = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var confirmCodeCallbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = confirmCode }, protocol: Request.Url.Scheme);
                    #region //create message body
                    StringBuilder message = new StringBuilder();
                    // generating the message
                    message.Append("Mr " + user.FullName + " ! You Need to Confirm your Email before you can reset your password");
                    message.Append(".<br/>");
                    message.AppendLine(" Please confirm your account by clicking <a href=\"" + confirmCodeCallbackUrl + "\">here</a><br/>");
                    #endregion
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account So you can reset your password", message.ToString());
                    ModelState.AddModelError("", "You need to confirm your account before resetting password , w've sent you a new confirmation email , Check your Email To see it");
                    return View(model);
                }
                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }



        [Authorize]
        // GET: Account/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [Authorize(Roles = "teacher")]
        // GET: Account/Edit/5
        public ActionResult Edit(string id)
        {
            //edit user get method , we take user id search for the user "handle id null and user null errors", 
            var userdb = ApplicationDbContext.Create();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = userdb.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var roleStore = new RoleStore<IdentityRole>(userdb);
            var roleMngr = new RoleManager<IdentityRole>(roleStore);

            availAbleRoles = roleMngr.Roles.Select(r => new SelectListItem() { Value = r.Id, Text = r.Name });
            avilableCourses = userdb.Courses.Where(c => c.EndDate > DateTime.Today)
                .Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name });
            var edituser = new EditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Courses = avilableCourses,
                Roles = availAbleRoles,
                SelectedCourse = user.CourseId,
                SelectedRole = user.Roles.FirstOrDefault().RoleId
            };
            return View(edituser);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "teacher")]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,UserName,PhoneNumber,SelectedRole,SelectedCourse")] EditViewModel user)
        {
            var userdb = ApplicationDbContext.Create();
            var roleStore = new RoleStore<IdentityRole>(userdb);
            var roleMngr = new RoleManager<IdentityRole>(roleStore);
            var roleName = roleMngr.FindById(user.SelectedRole).Name;

            //checking for non valid entries
            #region
            if (roleName == "teacher" && user.SelectedCourse != null)
            {
                ModelState.AddModelError("", "The teacher could't have a course");
            }
            if (roleName == "student" && (user.SelectedCourse == null || !userdb.Courses.Select(c => c.Id).Contains(user.SelectedCourse.Value)))
            {
                ModelState.AddModelError("", "the student should have a course");
            }
            if (!roleMngr.RoleExists(roleName))
            {
                ModelState.AddModelError("", "Invalid Role");
            }
            #endregion
            //update the user
            if (ModelState.IsValid)
            {
                var updatedUser = UserManager.FindById(user.Id);
                updatedUser.Email = user.Email;
                updatedUser.FirstName = user.FirstName;
                updatedUser.LastName = user.LastName;
                updatedUser.UserName = user.UserName;
                updatedUser.PhoneNumber = user.PhoneNumber;
                updatedUser.CourseId = user.SelectedCourse;
                UserManager.AddToRole(updatedUser.Id, roleName);
                UserManager.RemoveFromRoles(updatedUser.Id, roleStore.Roles.Where(r => r.Name != roleName).Select(r => r.Name).ToArray());
                UserManager.Update(updatedUser);
                return RedirectToAction("Details", "Account", new { id = user.Id });
            }

            //if something went wrong we need to recreate lists of roles and courses and pass it to the view
            #region
            availAbleRoles = roleMngr.Roles.Select(r => new SelectListItem() { Value = r.Id, Text = r.Name });
            avilableCourses = userdb.Courses.Where(c => c.EndDate > DateTime.Today)
                .Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name });
            user.Courses = avilableCourses;
            user.Roles = availAbleRoles;
            #endregion
            return View(user);
        }

        // GET: Account/Delete/5
        [Authorize(Roles = "teacher")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "teacher")]
        public ActionResult DeleteConfirmed(string id)
        {
            var userToDelete = db.Users.Find(id);
            var roleId = userToDelete.Roles.FirstOrDefault()?.RoleId;
            var roleName = db.Roles.Find(roleId);
            db.Users.Remove(userToDelete);
            db.SaveChanges();
            if (roleName.Name != null && roleName.Name.Equals("teacher"))
            {
                return RedirectToAction("Teachers", "Account");
            }
            return RedirectToAction("Students", "Account");
        }

        [NonAction]
        public string PasswordGenerator()
        {
            var password = new List<char>(6);
            char[] charArr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int passlong = 0;
            while (passlong < 6)
            {
                Random random = new Random();
                var randomIndex = random.Next(62);
                if (!password.Contains(charArr[randomIndex]))
                {
                    password.Add(charArr[randomIndex]);
                    passlong++;
                }
            }
            return new string(password.ToArray());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            var userId = SignInManager
                .AuthenticationManager
                .AuthenticationResponseGrant.Identity.GetUserId();
            var courseId = db.Users.Find(userId)?.CourseId;

            if (courseId == null)
            {
                return RedirectToAction("Index", "Courses");
            }

            return RedirectToAction("Details", "Courses", new { id = courseId });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
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
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
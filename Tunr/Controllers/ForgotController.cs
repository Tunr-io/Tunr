using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using TinyIoC;
using Tunr.Models.BindingModels;
using Tunr.Services;

namespace Tunr.Controllers
{
    public class ForgotController : Controller
    {
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
        private ApplicationUserManager _userManager;

        // GET: Forgot
        [HttpGet]
        public ActionResult Index()
        {
            var forgotModel = new ForgotPasswordBindingModel();
            return View(forgotModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ForgotPasswordBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var confirmationCode = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Forgot", new { userId = user.Id, code = confirmationCode }, protocol: Request.Url.Scheme);

                    // Send them a welcome mail!
                    string bodyText = "Hey " + user.DisplayName + ","
                        + "\n\nSomeone told us you forgot your password."
                        + "\nIf you did, just click the link below and you can set a new one."
                        + "\n\n" + callbackUrl
                        + "\n\nOtherwise, you can ignore this email."
                        + "\n\nIf you have any trouble, want to share your thoughts, or just want to talk, shoot us an email at support@tunr.io . We'd love to hear from you."
                        + "\n\n We <3 Music!"
                        + "\n\n The Tunr team";

                    try
                    {
                        TinyIoCContainer.Current.Resolve<IEmailService>().SendEmail("support@tunr.io", "Tunr Support", new List<string>() { user.Email }, "Password Reset", bodyText);
                    }
                    catch (Exception) { }
                }
                return View("ForgotPasswordConfirmation");
            }
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View();
            }
            var model = new PasswordResetBindingModel()
            {
                UserId = userId,
                ResetToken = code
            };
            return View("ResetPassword", model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(PasswordResetBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ResetPasswordAsync(model.UserId, model.ResetToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return View("ResetSuccess");
                } else
                {
                    return View("Error");
                }
            }
            return View("ResetPassword", model);
        }
    }
}
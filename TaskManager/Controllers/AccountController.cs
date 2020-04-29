using System.Web.Mvc;
using TaskManager.Helper;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize]
    [RoutePrefix("Account")]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // can retrieve from web.config as well but FYC i am holding the login details here itself.
            if (model.UserName == "robert" && model.Password == "robert")
            {
                System.Web.HttpContext.Current.Session.Add(ConstantHelper.UserSessionKey, "1");
                System.Web.HttpContext.Current.Session.Add(ConstantHelper.UserNameSessionKey, "Robert");
                return RedirectToLocal(returnUrl);
            }
            else if (model.UserName == "mccal" && model.Password == "mccal")
            {
                System.Web.HttpContext.Current.Session.Add(ConstantHelper.UserSessionKey, "2");
                System.Web.HttpContext.Current.Session.Add(ConstantHelper.UserNameSessionKey, "Mccal");
                return RedirectToLocal(returnUrl);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion Helpers
    }
}
using System.Web.Mvc;
using TaskManager.Helper;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current != null &&
                System.Web.HttpContext.Current.Session[ConstantHelper.UserSessionKey] != null)
            {
                ViewBag.Title = "Home Page";

                return View("Index");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

using TaskManager.Controllers;
using TaskManager.Helper;
using NUnit.Framework;

namespace TaskManager.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        private HomeController controller;

        [SetUp]
        public void TestSetup()
        {
            InitialiseSessionMocks();
            InitialiseControllerMocks();
        }

        [Test]
        public void BasicTest_Push_Item_Into_Session()
        {
            // Arrange
            var itemValue = "1";
            var itemKey = ConstantHelper.UserSessionKey;

            // Act
            HttpContext.Current.Session.Add(itemKey, itemValue);

            // Assert
            Assert.AreEqual(HttpContext.Current.Session[itemKey], itemValue);
        }

        [Test]
        public void HomeController_Redirects_AccountLoginAction()
        {
            // Arrange
            var expectedController = "Account";
            var expectedAction = "Login";

            // Act
            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
        }

        [Test]
        public void HomeController_Returns_HomeIndexAction_WhenSessionIsSet()
        {
            // Arrange
            var expectedViewName = "Index";
            var itemValue = "1";
            var itemKey = ConstantHelper.UserSessionKey;

            // Act
            HttpContext.Current.Session.Add(itemKey, itemValue);
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual(expectedViewName, result.ViewName);
        }

        private void InitialiseSessionMocks()
        {
            // We need to setup the Current HTTP Context as follows:

            // Step 1: Setup the HTTP Request
            var httpRequest = new HttpRequest("", "http://localhost:63586", "");

            // Step 2: Setup the HTTP Response
            var httpResponce = new HttpResponse(new StringWriter());

            // Step 3: Setup the Http Context
            var httpContext = new HttpContext(httpRequest, httpResponce);
            var sessionContainer = new HttpSessionStateContainer("FakeSessionId",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10,
                true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc,
                false);
            httpContext.Items["AspSession"] = typeof(HttpSessionState)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    CallingConventions.Standard,
                    new[] { typeof(HttpSessionStateContainer) },
                    null)
                .Invoke(new object[] { sessionContainer });

            // Step 4: Assign the Context
            HttpContext.Current = httpContext;
        }

        private void InitialiseControllerMocks()
        {
            controller = new HomeController();
        }
    }
}
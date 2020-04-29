using System.Web;
using System.Web.Optimization;

namespace TaskManager
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                  "~/Scripts/angular.js",
                  "~/Scripts/angular-messages.js",
                  "~/Scripts/angular-route.js",
                  "~/Scripts/angular-animate.js",
                  "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                  "~/Scripts/trNgGrid.js",
                  "~/Scripts/angular.treeview.js",
                  "~/Scripts/angular-resource.js",
                  "~/Scripts/angular-sanitize.js",
                   "~/Scripts/moment.js"
                 ));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                "~/Scripts/task/appMain.js",
                 "~/Scripts/task/constants.js"
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
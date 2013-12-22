using System.Web;
using System.Web.Optimization;

namespace WebUi
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                  "~/Scripts/ckeditor/ckeditor.js",
                  "~/Scripts/ckeditor/adapters/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new StyleBundle("~/Content/Css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/business-casual.css",
                      "~/Content/Site.css",
                      "~/Content/font-awesome.css"));
        }
    }
}

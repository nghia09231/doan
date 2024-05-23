using System.Web;
using System.Web.Optimization;

namespace Fahasa
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/style.css"));

            bundles.Add(new StyleBundle("~/Content/cssDetail").Include( "~/Content/product-detail.css"));
            bundles.Add(new StyleBundle("~/Content/cssCheckout").Include( "~/Content/checkout.css"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
           "~/Scripts/knockout-{version}.js",
           "~/Scripts/app.js"));

            bundles.Add(new StyleBundle("~/bundle/admin/login/css").Include(
                                            "~/Areas/Admin/Content/css/style.css"
            ));
            bundles.Add(new ScriptBundle("~/bundle/admin/login/js").Include(
                                "~/Areas/Admin/Content/assets/libs/popper.js/dist/umd/popper.min.js",
                                "~/Areas/Admin/Content/assets/libs/bootstrap/dist/js/bootstrap.min.js"
            ));
            bundles.Add(new ScriptBundle("~/bundle/adminJs").Include(
                                "~/Areas/Admin/Content/assets/libs/popper.js/dist/umd/popper.min.js",
                                "~/Areas/Admin/Content/assets/libs/bootstrap/dist/js/bootstrap.min.js",
                                "~/Areas/Admin/Content/js/app-style-switcher.js",
                                "~/Areas/Admin/Content/js/feather.min.js",
                                "~/Areas/Admin/Content/assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js",
                                "~/Areas/Admin/Content/js/sidebarmenu.js",
                                "~/Areas/Admin/Content/js/custom.min.js",
                                "~/Areas/Admin/Content/assets/extra-libs/c3/d3.min.js",
                                "~/Areas/Admin/Content/assets/extra-libs/c3/c3.min.js",
                                "~/Areas/Admin/Content/assets/extra-libs/jvector/jquery-jvectormap-2.0.2.min.js",
                                "~/Areas/Admin/Content/assets/extra-libs/jvector/jquery-jvectormap-world-mill-en.js",
                                "~/Areas/Admin/Content/js/pages/dashboards/dashboard1.min.js",
                                "~/Areas/Admin/Content/assets/extra-libs/sparkline/sparkline.js"
            ));
            bundles.Add(new StyleBundle("~/bundle/adminCss").Include(
                                           "~/Areas/Admin/Content/assets/extra-libs/c3/c3.min.css",
                                           "~/Areas/Admin/Content/assets/libs/chartist/dist/chartist.min.css",
                                           "~/Areas/Admin/Content/assets/extra-libs/jvector/jquery-jvectormap-2.0.2.css",
                                           "~/Areas/Admin/Content/css/style.css"
           ));
        }
    }
}

/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BundleConfig.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System.Web;
using System.Web.Optimization;

namespace Senparc.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                      "~/Scripts/AngularJS/angular.js",
                      "~/Scripts/AngularJS/angular-route.js",
                      "~/Scripts/AngularJS/angular-touch.js",
                      "~/Scripts/AngularJS/angular-animate.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/globaljs").Include(
                    "~/Scripts/global.js",
                    "~/Scripts/hammer.min.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/wx/js").Include(
                    "~/Scripts/vue/vue.js",
                    "~/Scripts/vue/plugin/vue-touch.js",
                    "~/Scripts/vue/plugin/vue-tap.js"
                ));
            //bundles.Add(new ScriptBundle("~/bundles/wx/css").Include(
            //        "~/Areas/WX/Content/css/weui.css"
            //    ));
        }
    }
}

/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AdminAreaRegistration.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System.Web.Mvc;
using System.Web.Optimization;

namespace Senparc.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SenparcWeixinMarketingAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "SenparcWeixinMarketingAdmin/{controller}/{action}/{id}",//这里的SenparcWeixinMarketingAdmin可以根据需要修改成更加复杂的路径，当然实际项目中，为了提高安全性Admin模块可以不用部署
                new { action = "Index", controller = "Home", id = UrlParameter.Optional }
            );

            RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/backend/css/bootstrap").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/bootstrap.min.css"
                //"~/Content/app.css"
                ));


            bundles.Add(new StyleBundle("~/backend/css/bootstrap-rtl").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/bootstrap-rtl.min.css"));

            bundles.Add(new StyleBundle("~/backend/css/beyond").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/beyond.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/demo.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/font-awesome.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/typicons.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/weather-icons.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/animate.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/dataTables.bootstrap.css",
                "~/Content/summernote/summernote.css",
                //"~/Scripts/Angular-Extension/angular-ui-select/select.css",
                "~/Content/pagecss.css",
                "~/Scripts/Angular-Extension/angular-toaster/toaster.css"
                //"~/Scripts/Angular-Extension/ng-tags-input/ng-tags-input.css"
                ));

            bundles.Add(new StyleBundle("~/backend/css/beyond-rtl").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/beyond-rtl.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/demo.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/font-awesome.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/typicons.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/weather-icons.min.css",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/css/animate.min.css"
                ));

            bundles.Add(new ScriptBundle("~/backend/bundles/js").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/skins.min.js"
              //, "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/jquery-2.0.3.min.js"
               , "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/bootstrap.min.js"
               , "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/slimscroll/jquery.slimscroll.min.js"
                , "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/beyond.js"
                , "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/select2/select2.js"
                , "~/Scripts/common.js"
                , "~/Scripts/underscore/underscore.js"
                ));

            bundles.Add(new ScriptBundle("~/backend/angularjs/app").Include(
                "~/Scripts/Angular-Extension/ui-bootstrap.js"
                , "~/Scripts/Angular-Extension/fileUploader/fileUploader.js"
                , "~/Scripts/Angular-Extension/angular-ivh-treeview/ivh-treeview.js"
                , "~/Scripts/Angular-Extension/angular-summernote.js"
                //, "~/Scripts/Angular-Extension/angular-ui-select/select.min.js"
                , "~/Scripts/Angular-Extension/angular-toaster/toaster.js"
                //, "~/Scripts/Angular-Extension/ui.bootstrap.contextMenu.js"
                //, "~/Scripts/Angular-Extension/angular-minicolors/jquery.minicolors.min.js"
                //, "~/Scripts/Angular-Extension/angular-minicolors/angular-minicolors.js"
                //, "~/Scripts/Angular-Extension/angular-ui-utils/angular-ui-utils.js"
                //, "~/Scripts/Angular-Extension/ng-tags-input/ng-tags-input.js"
                //, "~/Scripts/Angular-Extension/ng-infinite-scroll.js"
                , "~/Areas/SenparcWeixinMarketingAdmin/Scripts/AngularJS/app.js"));

            bundles.Add(new ScriptBundle("~/backend/bundles/beyond").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/beyond.min.js"));

            bundles.Add(new ScriptBundle("~/backend/bundles/skin").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/skins.min.js"));

            bundles.Add(new ScriptBundle("~/backend/bundles/jquery").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/jquery-2.0.3.min.js"));

            bundles.Add(new ScriptBundle("~/backend/bundles/bootstrap").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/bootstrap.min.js",
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/slimscroll/jquery.slimscroll.min.js"
                ));

            bundles.Add(new ScriptBundle("~/backend/summernote").Include(
                "~/Content/summernote/summernote.js"
                , "~/Content/summernote/lang/summernote-zh-CN.js"
                , "~/Content/summernote/plugin/summernote-ext-video.js"
                , "~/Content/summernote/plugin/summernote-ext-emoji.js"
                , "~/Content/summernote/plugin/summernote-ext-hint.js"
                , "~/Content/summernote/plugin/summernote-ext-specialchar.js"
                , "~/Content/summernote/plugin/summernote-ext-hello.js"
                , "~/Content/summernote/plugin/summernote-ext-chooseColor.js"
                ));

            bundles.Add(new ScriptBundle("~/backend/bootbox").Include(
                "~/Areas/SenparcWeixinMarketingAdmin/Content/backend/js/bootbox/bootbox.js"));
        }
    }
}

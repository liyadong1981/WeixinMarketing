/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：WXAreaRegistration.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System.Web.Mvc;
using System.Web.Optimization;

namespace Senparc.Areas.WX
{
    public class WXAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WX";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WX_default",
                "WX/{controller}/{action}/{id}",
                new { action = "Index", controller = "Home", id = UrlParameter.Optional }
            );

            RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/wx/js").Include(
                         "~/Scripts/zepto/zepto.js",
                         "~/Scripts/underscore/underscore.js",
                         "~/Content/pgwmodal/pgwmodal.js",
                         "~/Scripts/zepto.dpToast.js",
                         "~/Areas/WX/Content/swiper/js/swiper.min.js",
                         "~/Scripts/vue/vue.js",
                         "~/Scripts/vue/plugin/vue-touch.js",
                         "~/Areas/WX/Scripts/jquery-1.9.1.min.js",
                //"~/Areas/WX/Scripts/product/address_home.js",
                //"~/Areas/WX/Scripts/product/order_home.js",
                //"~/Areas/WX/Scripts/product/shopcart_home.js",
                //"~/Areas/WX/Scripts/product/product_home.js",
                //"~/Areas/WX/Scripts/product/points_home.js",
                          "~/Areas/WX/Scripts/product/page.js",
                          "~/Areas/WX/Content/css/weui/example.js"
                        ));
            bundles.Add(new StyleBundle("~/bundles/weui/css").Include(
                        "~/Areas/WX/Content/css/weui/weui.css",
                        "~/Areas/WX/Content/css/weui/example.css",
                        "~/Areas/WX/Content/css/weui/example.js"

                       ));
            bundles.Add(new StyleBundle("~/bundles/wx/css").Include(
                //"~/Areas/WX/Content/css/style.min.css"
                //, "~/Content/pgwmodal/pgwmodal.css",
                         "~/Areas/WX/Content/css/main.css",
                          "~/Areas/WX/Content/swiper/css/swiper.min.css",
                          "~/Areas/WX/Content/swiper/css/weui/example.css",
                          "~/Areas/WX/Content/swiper/css/weui/weui.css"

                        ));

            bundles.Add(new StyleBundle("~/bundles/mobiscroll/css").Include(
                        "~/Content/mobiscroll/css/mobiscroll.icons.css",
                        "~/Content/mobiscroll/css/mobiscroll.widget.css",
                        "~/Content/mobiscroll/css/mobiscroll.widget.android-holo.css",
                        "~/Content/mobiscroll/css/mobiscroll.scroller.css",
                        "~/Content/mobiscroll/css/mobiscroll.scroller.android-holo.css"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/mobiscroll/js").Include(
                        "~/Content/mobiscroll/js/mobiscroll.zepto.js",
                        "~/Content/mobiscroll/js/mobiscroll.core.js",
                        "~/Content/mobiscroll/js/mobiscroll.util.datetime.js",
                        "~/Content/mobiscroll/js/mobiscroll.widget.js",
                        "~/Content/mobiscroll/js/mobiscroll.scroller.js",
                        "~/Content/mobiscroll/js/mobiscroll.datetime.js",
                        "~/Content/mobiscroll/js/mobiscroll.select.js",
                        "~/Content/mobiscroll/js/mobiscroll.widget.android-holo.js",
                        "~/Content/mobiscroll/js/i18n/mobiscroll.i18n.zh.js"
                        ));

        }
    }
}

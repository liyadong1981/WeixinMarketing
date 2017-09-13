/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BackendTemplateExtension.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Senparc.Core.Extensions;
using Senparc.Core.Models.VD;
using Senparc.Core.Enums;

namespace Senparc.Web.UI
{
    public static class BackendTemplateExtension
    {
        public static ContentBox ContentBox(this HtmlHelper htmlHelper, string title, params object[] tabs)
        {
            return ContentBox(htmlHelper, title, true, tabs);
        }

        public static ContentBox ContentBox(this HtmlHelper htmlHelper, string title, bool showDefaultTabContainer, params object[] tabs)
        {
            StringBuilder tabsCollection = new StringBuilder();
            foreach (var tab in tabs)
            {
                tabsCollection.AppendFormat("{0}", tab);
            }

            var header = (@"
<div class=""widget"">
    <div class=""widget-header with-footer"">
        <span class=""widget-caption"">{0}</span>
        <div class=""widget-buttons {2}"">
            {1}
        </div>
    </div>
<div class=""widget-body"">")
.With(title, tabsCollection, tabsCollection.Length == 0 ? "hide" : "");
            htmlHelper.ViewContext.Writer.Write(header);

            ContentBox contentBox = new ContentBox(htmlHelper.ViewContext, showDefaultTabContainer);
            return contentBox;
        }

        public static MvcHtmlString RenderMessageBox(this HtmlHelper helper)
        {
            var model = helper.ViewData.Model as IBaseUiVD;
            if (model == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();


            if (model.MessagerList != null && model.MessagerList.Count > 0)
            {
                foreach (var item in model.MessagerList)
                {
                    sb.Append(helper.MessageBox(item.MessageType, item.MessageText));
                }
            }
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString MessageBox(this HtmlHelper helper, MessageType messageType, string messageText, bool showClose = true)
        {
            string format = @"
<div class=""alert alert-{0}"">"
+ (showClose ? @"<button class=""close"" data-dismiss=""alert"">×</button>" : null) +
@"<i class=""fa-fw fa fa-{0}""></i>
{1}
</div>";
            return new MvcHtmlString(format.With(messageType.ToString(), messageText));
        }
    }
}

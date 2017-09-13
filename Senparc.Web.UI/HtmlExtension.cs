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
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Senparc.Core.Extensions;
using Senparc.Core.Utility;
using System.Linq.Expressions;
using Senparc.Mvc;

namespace Senparc.Web.UI
{
    public static class HtmlExtension
    {
        //#region CustomSelect
        ///// <summary>
        ///// 返回由枚举类型生成的自定义样式下拉选项
        ///// </summary>
        //public static MvcHtmlString CustomDropDownListFormEnum(this HtmlHelper htmlHelper, string name, Type enumType, object selectedValue = null, string optionLabel = null, object htmlAttributes = null, bool useDescription = false, bool addBlankOption = false, string blankOptionText = null)
        //{
        //    var selectedList = GetSelectListFromEnum(enumType, selectedValue, useDescription, addBlankOption, blankOptionText);
        //    StringBuilder result = new StringBuilder();
        //    foreach (var item in selectedList)
        //    {
        //        result.AppendFormat("<img src=\"/Images/up_un.gif\" />");
        //    }
        //    return htmlHelper.DropDownList(name, selectedList, optionLabel, htmlAttributes);
        //}
        //#endregion

        #region DropDownList

        /// <summary>
        /// 返回由枚举类型生成的下拉选项
        /// </summary>
        public static MvcHtmlString DropDownListFormEnumFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Type enumType, object selectedValue = null, string optionLabel = null, object htmlAttributes = null, bool useDescription = false, bool addBlankOption = false)
        {
            var selectedList = GetSelectListFromEnum(enumType, selectedValue, useDescription, addBlankOption);
            return htmlHelper.DropDownListFor(expression, selectedList, optionLabel, htmlAttributes);
        }
        /// <summary>
        /// 返回由枚举类型生成的下拉选项
        /// </summary>
        public static MvcHtmlString DropDownListFormEnum(this HtmlHelper htmlHelper, string name, Type enumType, object selectedValue = null, string optionLabel = null, object htmlAttributes = null, bool useDescription = false, bool addBlankOption = false, string blankOptionText = null)
        {
            var selectedList = GetSelectListFromEnum(enumType, selectedValue, useDescription, addBlankOption, blankOptionText);
            return htmlHelper.DropDownList(name, selectedList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString GetDescriptionForEnum(this HtmlHelper htmlHelper, Type enumType, int item)
        {
            return MvcHtmlString.Create(enumType.GetDescriptionForEnum(item));
        }

        private static SelectList GetSelectListFromEnum(Type enumType, object selectedValue, bool useDescription = false, bool addBlankOption = false, string blankOptionText = null)
        {
            var dic = Extensions.GetDictionaryForEnums(enumType, useDescription, addBlankOption, blankOptionText);
            return GetSelectListFromDictionary(dic, selectedValue);
        }

        private static SelectList GetSelectListFromDictionary(Dictionary<string, string> dic, object selectedValue)
        {
            return new SelectList(dic, "Key", "Value", selectedValue);
        }
        #endregion
    }
}

/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：GridViewActionExtension.cs

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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.IO;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Data;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public class GridViewActionItemTemplateModel<T>
    {
        public string Header { get; set; }
        public Action<T, int> ItemTemplate { get; set; }
        public Action<T> Footer { get; set; }
        public object HtmlAttributes { get; set; }
        //public Action<T> EditTemplate { get; set; }


        public GridViewActionItemTemplateModel() { }

        public GridViewActionItemTemplateModel(string header, Action<T, int> itemTemplate, Action<T> footer, object htmlAttributes)
        {
            this.Header = header;
            this.ItemTemplate = itemTemplate;
            this.Footer = footer;
            this.HtmlAttributes = htmlAttributes.ToAttributeList();
        }

        public GridViewActionItemTemplateModel(string header, Action<T, int> itemTemplate, object htmlAttributes)
            : this(header, itemTemplate, null, htmlAttributes)
        { }

        public GridViewActionItemTemplateModel(Action<T, int> itemTemplate)
            : this(null, itemTemplate, null)
        { }

        //public GridViewItemTemplateModel(Func<T, int, string> itemTemplate, Action<T> editTemplate)
        //{
        //    this.ItemTemplate = itemTemplate;
        //    this.EditTemplate = editTemplate;
        //}
    }

    public static class GridViewActionExtension
    {
        /// <summary>
        /// GridView
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="dataSource">数据源</param>
        /// <param name="htmlAttributes">Table的属性</param>
        /// <param name="emptyTemplete">没有数据时显示。没有数据时，Footer不显示</param>
        /// <param name="htmlCodeFormat">HTML代码换行（供调试状态下使用）。如果为false，自动生成的代码将不换行</param>
        /// <param name="itemTempletes">模板数据</param>
        /// <returns></returns>
        public static string GridViewAction<T>(this HtmlHelper helper, IEnumerable<T> dataSource,
            object htmlAttributes, string emptyTemplete, bool htmlCodeFormat, params GridViewActionItemTemplateModel<T>[] itemTempletes)
        {
            foreach (var item in dataSource)
            {
                foreach (var temp in itemTempletes)
                {
                    temp.ItemTemplate(item, 0);
                }
            }
            return "ddd";
        }
    }
}

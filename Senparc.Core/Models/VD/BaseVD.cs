/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseVD.cs

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
using System.Web.Routing;

namespace Senparc.Core.Models.VD
{
    public interface IBaseUiVD
    {
        FullAdminUserInfo FullAdminUserInfo { get; set; }
        FullSystemConfig FullSystemConfig { get; set; }
        string UserName { get; set; }
        bool IsAdmin { get; set; }
        string CurrentMenu { get; set; }
        List<Messager> MessagerList { get; set; }
        MetaCollection MetaCollection { get; set; }
        DateTime PageStartTime { get; set; }
        DateTime PageEndTime { get; set; }
    }

    public interface IBaseVD : IBaseUiVD
    {
        RouteData RouteData { get; set; }
    }

    public class BaseVD : IBaseVD
    {
        public FullAdminUserInfo FullAdminUserInfo { get; set; }
        public FullSystemConfig FullSystemConfig { get; set; }
        public MetaCollection MetaCollection { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public RouteData RouteData { get; set; }
        public string CurrentMenu { get; set; }
        public List<Messager> MessagerList { get; set; }
        public DateTime PageStartTime { get; set; }
        public DateTime PageEndTime { get; set; }
    }


    public class Base_PagerVD
    {
        public int? PageIndex { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }

        public Base_PagerVD(int? pageIndex, int pageCount, int totalCount)
        {
            this.PageIndex = pageIndex;
            this.PageCount = pageCount;
            this.TotalCount = totalCount;
        }
    }
    public class LoginBarVD
    {
        public bool Logined { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }

    public class SuccessVD : BaseVD
    {
        public string Message { get; set; }
        public string BackUrl { get; set; }
        public string BackAction { get; set; }
        public string BackController { get; set; }
        public RouteValueDictionary BackRouteValues { get; set; }
    }
}

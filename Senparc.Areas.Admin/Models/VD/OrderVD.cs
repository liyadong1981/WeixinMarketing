﻿/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：OrderVD.cs

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
using Senparc.Core.Models;

namespace Senparc.Areas.Admin.Models.VD
{
    public class BaseOrderVD : BaseAdminVD
    { 
    }

    public class Order_IndexVD : BaseOrderVD
    {
        public PagedList<Order> OrderList { get; set; }

        public Core.Enums.Order_Status? CurrentOrderState { get; set; }
    }

    public class Order_EditVD : BaseOrderVD
    {
        public Order Order { get; set; }
        public bool IsEdit { get; set; }
    }

    public class Order_ItemVD : BaseOrderVD
    {
        public Order Order { get; set; }
    }
}

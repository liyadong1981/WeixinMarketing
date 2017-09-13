/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：OrderController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Web.Mvc;
using Senparc.Areas.Admin.Models.VD;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Mvc.Filter;
using Senparc.Service;

namespace Senparc.Areas.Admin.Controllers
{
    [MenuFilter("Order")]
    public class OrderController : BaseAdminController
    {
        private OrderService _orderService;
        private APP_RedPackage_ActivityService _appRedPackageActivityService;
        public OrderController(OrderService orderService, APP_RedPackage_ActivityService appRedPackageActivityService)
        {
            _orderService = orderService;
            _appRedPackageActivityService = appRedPackageActivityService;
        }

        public ActionResult Index(int pageIndex = 1, Order_Status? state = null)
        {
            var pageCount = 20;

            var orderList = _orderService.GetPagedOrderList(pageIndex, pageCount, state);

            var vd = new Order_IndexVD()
            {
                OrderList = orderList,
                CurrentOrderState = state
            };
            return View(vd);
        }

        public ActionResult Item(int id)
        {
            var order = _orderService.GetObject(z => z.Id == id, new[] { "OrderItems" });
            if (order == null)
            {
                return RenderError("订单不存在！");
            }

            var vd = new Order_ItemVD()
            {
                Order = order,
            };

            return View(vd);
        }

        public ActionResult Edit(int id)
        {
            var order = _orderService.GetObject(id);
            if (order == null)
            {
                return RenderError("订单不存在！");
            }

            var vd = new Order_EditVD()
            {
                Order = order,
                IsEdit = true
            };
            return View(vd);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Prefix = "Order")] Order order_Form)
        {
            if (!ModelState.IsValid)
            {
                Order_EditVD vd = new Order_EditVD()
                {
                    Order = order_Form,
                    IsEdit = true,
                };
                return View(vd);
            }

            Order order = null;

            order = _orderService.GetObject(order_Form.Id);
            if (order == null)
            {
                return RenderError("订单不存在！");
            }

            //后台订单添加物流单号，订单状态改成待收货
            TryUpdateModel(order, "Order", null, new[] { "Id" });
            _orderService.SaveObject(order);

            SetMessager(MessageType.success, "订单修改成功！");
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var order = _orderService.GetObject(id);
            if (order == null)
            {
                return RenderError("订单不存在！");
            }
            try
            {
                _orderService.DeleteObject(order);
            }
            catch (Exception ex)
            {
                return RenderError(ex.Message);
            }

            SetMessager(MessageType.success, "删除成功");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult GetOrderState(int orderId)
        {
            var order = _orderService.GetObject(z => z.Id == orderId);
            if (order == null)
            {
                return RenderError("订单不存在!!");
            }
            return RenderJsonSuccessResult(true, new { State = order.Status, Money = order.TotalPrice });
        }

        [HttpPost]
        public ActionResult TenPay(int id, decimal money)
        {
            var activity = _appRedPackageActivityService.GetObject(z => z.Id == id);
            if (activity == null)
            {
                return RenderError("活动不存在！！");
            }
            //创建订单
            var order = _orderService.AddOrder(id, "", money, money, activity.Description,
                  Order_Type.摇一摇红包);
            try
            {
                var codeUrl = _orderService.TenPayV3QrCodeInfo(order, Request.UserHostAddress);
                return RenderJsonSuccessResult(true, new { OrderId = order.Id, CodeUrl = codeUrl });
            }
            catch (Exception ex)
            {
                LogUtility.Order.Error("二维码获取出错：{0}".With(ex.Message), ex);
                return RenderJsonSuccessResult(true, new { Message = "二维码获取出错" });
            }
        }

    }
}
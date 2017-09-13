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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Senparc.Areas.WX.Models.VD;
using Senparc.Core.Cache;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Mvc;
using Senparc.Mvc.Filter;
using Senparc.Service;
using StructureMap;

namespace Senparc.Areas.WX.Controllers
{
    public class OrderController : BaseWXController
    {
        //private IOrderService _orderService;
        //IProductService _productService;
        //private ICouponService _couponService;

        /// <summary>
        /// 获取订单JSON
        /// </summary>
        /// <returns></returns>
        //        private object GetOrderJson(Order order)
        //        {
        //            return new
        //            {
        //                order.Id,
        //                order.OrderNumber,
        //                order.TotalPrice,
        //                State = ((Order_State)order.State).ToString(),
        //                AddTime = order.AddTime.ToString("yyyy-MM-dd HH:mm"),
        //                OrderItems = order.OrderItems.Select(z => new
        //                {
        //                    z.ProductId,
        //                    z.Id,
        //                    z.ProductName,
        //                    z.ProductPrice,
        //                    z.ProductPicUrl,
        //                    z.Count,
        //                    AddTime = z.AddTime.ToString("yyyy-MM-dd HH:mm"),
        //                })
        //            };
        //        }

        //        public OrderController(IOrderService orderService, IAddressService addressService, IProductService productService, ICouponService couponService)
        //        {
        //            _orderService = orderService;
        //            _addressService = addressService;
        //            _productService = productService;
        //            _couponService = couponService;
        //        }

        public ActionResult Index()
        {
            Order_IndexVD vd = new Order_IndexVD();

            return View(vd);
        }

        public ActionResult Item(int id)
        {
            //            var order = _orderService.GetObject(z => z.Id == id && z.AccountId == FullAccount.Id);
            //            if (order == null)
            //            {
            //                return RenderError("订单信息不存在！");
            //            }
            //
            //            Order_ItemVD vd = new Order_ItemVD()
            //            {
            //                Order = order
            //            };
            //
            //            return View(vd);
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrder(string productIds, string counts)
        {
            //var productIdList = productIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            //     .Select(z => int.Parse(z)).ToList();
            //var countList = counts.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            //    .Select(z => int.Parse(z)).ToList();
            //
            //if (productIds.Length < 0)
            //{
            //    return RenderJsonSuccessResult(false, "您没有选择任何商品，无法进行支付！");
            //}
            //
            //var shortProductList = new List<ShortProduct>();
            //for (int i = 0; i < productIdList.Count; i++)
            //{
            //    var productId = productIdList[i];
            //    var count = countList[i];
            //    shortProductList.Add(new ShortProduct()
            //    {
            //        ProductId = productId,
            //        Count = count
            //    });
            //}
            //
            //var address = _addressService.GetDefaultAddress(FullAccount.Id);
            //
            //var order = _orderService.AddOrder(FullAccount.Id, shortProductList, address, null);
            //return RenderJsonSuccessResult(true, Url.Action("SureOrder", new { id = order.Id }));
            return RenderJsonSuccessResult(true, new { }, true);
        }

        public ActionResult SureOrder(int id)
        {
            //var order = _orderService.GetObject(z => z.Id == id && z.AccountId == FullAccount.Id);
            //if (order == null)
            //{
            //    return RenderError("订单信息不存在！");
            //}
            //
            ////var couponService = ObjectFactory.GetInstance<ICouponService>();
            //var vd = new Order_SureOrderVD()
            //{
            //    Order = order,
            //    Coupon = _orderService.GetValiableCoupon(id, FullAccount.Id)
            //};
            //return View(vd);
            return View();
        }

        public ActionResult ChangeAddress(int addressId, int orderId)
        {
            //var order = _orderService.GetObject(z => z.Id == orderId && z.AccountId == FullAccount.Id);
            //if (order == null)
            //{
            //    return RenderError("订单信息不存在！");
            //}
            //
            //var address = _addressService.GetObject(z => z.Id == addressId && z.AccountId == FullAccount.Id);
            //if (address == null)
            //{
            //    return RenderError("地址信息不存在！");
            //}
            //
            //_orderService.ChangeAddress(order, address);
            //
            //_orderService.CaculateOrderRealPrice(order, true);//重新计算订单金额等

            return RedirectToAction("SureOrder", new { id = orderId });
        }

        /// <summary>
        /// 支付成功页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PaySuccess(int id)
        {
            //var order = _orderService.GetObject(z => z.Id == id && z.AccountId == FullAccount.Id);
            //if (order == null)
            //{
            //    return RenderError("订单信息不存在！");
            //}
            //
            //var couponList = order.Coupons;
            //if (couponList.Count(z => !z.AccountId.HasValue) == 0)
            //{
            //    //TODO:直接转到转发页面
            //    return RedirectToAction("Share", "Coupon", new { orderId = id });
            //    //return RenderError("当前订单的优惠券已发到您账户中，请注意查看！");
            //}
            //
            //var fullSystemConfigCache = ObjectFactory.GetInstance<IFullSystemConfigCache>();
            //var fullSystemConfig = fullSystemConfigCache.Data;
            //
            //Order_PaySuccessVD vd = new Order_PaySuccessVD()
            //{
            //    Order = order,
            //    FullSystemConfig = fullSystemConfig
            //};
            //
            //return View(vd);
            return View();
        }


        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="lastId"></param>
        /// <param name="state">订单状态</param>
        /// <returns></returns>
        public ActionResult GetOrderList(int lastId = 0, Order_Status? state = null)
        {
            //int pageCount = 15;
            //
            //var orderList = _orderService.GetOrderList(lastId, pageCount, FullAccount.Id, state, false);
            //
            //return RenderJsonSuccessResult(true, new
            //{
            //    OrderList = orderList.Select(z => GetOrderJson(z))
            //
            //}, true);
            return RenderJsonSuccessResult(true, new { }, true);
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetOrder(int id)
        {
            //var order = _orderService.GetObject(z => z.Id == id && z.AccountId == FullAccount.Id, new[] { "OrderItems" });
            //if (order == null)
            //{
            //    return RenderJsonSuccessResult(false, "订单不存在", true);
            //}
            //
            //return RenderJsonSuccessResult(true, GetOrderJson(order), true);
            return RenderJsonSuccessResult(true, new { }, true);
        }

        [HttpPost]
        public ActionResult ReadyToPay(int id)
        {
            //var order =
            //   _orderService.GetObject(z => z.Id == id && z.AccountId == FullAccount.Id);
            //if (order == null)
            //{
            //    return RenderJsonSuccessResult(false, "订单不存在", true);
            //}
            //
            //_orderService.ReadyToPay(order);
            return RenderJsonSuccessResult(true, "Ready to pay.");
        }

        /// <summary>
        /// 微信支付参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TenPay(int id)
        {
            //var order =
            //    _orderService.GetObject(z => z.Id == id && z.AccountId == FullAccount.Id, new[] { "OrderItems" });
            //if (order == null)
            //{
            //    return RenderError("抱歉，未找到订单！");
            //}
            //
            //if (order.State != (int)Order_State.未支付)
            //{
            //    return RenderError("当前订单不能支付");
            //}
            //
            //try
            //{
            //    //tenPayV3Info 等同命名： tenPayV3JsApi
            //    var tenPayV3Info = _orderService.TenPayV3Info(order, Request.UserHostAddress, FullAccount.WeixinOpenId);
            //
            //    return RenderJsonSuccessResult(true, new
            //    {
            //        TenPayV3Info = tenPayV3Info,
            //        order.Id,
            //        order.Message,
            //        order.OrderNumber,
            //        AddTime = order.AddTime.ToShortTime(),
            //        order.Address,
            //        order.AccountId,
            //        order.Province,
            //        order.City,
            //        order.District,
            //        order.PayType,
            //        order.Contacter,
            //        order.Phone,
            //        order.TrackNumber,
            //        order.TotalPrice,
            //        OrderItems = order.OrderItems.Select(z => new
            //        {
            //            z.ProductId,
            //            z.Id,
            //            z.ProductName,
            //            z.ProductPrice,
            //        })
            //    });
            //}
            //catch (Exception ex)
            //{
            //    return RenderJsonSuccessResult(false, new { Message = ex.Message });
            //}
            return RenderJsonSuccessResult(false, new { });
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelOrder(int id)
        {
            //var order = _orderService.GetObject(z => z.Id == id && z.AccountId == FullAccount.Id, new[] { "OrderItems" });
            //if (order == null)
            //{
            //    return RenderJsonSuccessResult(false, "订单不存在");
            //}
            //
            //if (order.State != (int)Order_State.未支付)
            //{
            //    return RenderJsonSuccessResult(false, "当前订单不能取消！");
            //}
            //
            //try
            //{
            //    //取消订单
            //    _orderService.CancelOrder(order);
            //}
            //catch (Exception ex)
            //{
            //    return RenderJsonSuccessResult(false, ex.Message);
            //}

            return RenderJsonSuccessResult(true, "取消成功！");
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReceiveOrder(int orderId)
        {
            //var order = _orderService.GetObject(z => z.Id == orderId && z.AccountId == FullAccount.Id, new[] { "OrderItems" });
            //if (order == null)
            //{
            //    return RenderJsonSuccessResult(false, "订单不存在");
            //}
            //
            //try
            //{
            //    //确认收货
            //    _orderService.ReceiveOrder(order);
            //}
            //catch (Exception ex)
            //{
            //    return RenderJsonSuccessResult(false, ex.Message);
            //}

            return RenderJsonSuccessResult(true, new { Message = "操作成功！" });
        }
    }
}

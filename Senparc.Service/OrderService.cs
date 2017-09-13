/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：OrderService.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Pay.TenPay;
using Senparc.Repository;
using Senparc.Log;
using Senparc.Utility;
using StructureMap;

namespace Senparc.Service
{
    public class OrderService : BaseClientService<Order> //, IOrderService
    {
        public OrderService(OrderRepository orderRepo)
            : base(orderRepo)
        {
        }

        public PagedList<Order> GetPagedOrderList(int pageIndex, int pageCount, Order_Status? status)
        {
            var seh = new SenparcExpressionHelper<Order>();

            seh.ValueCompare
                .AndAlso(status.HasValue, z => z.Status == (int)status);
            var where = seh.BuildWhereExpression();


            return GetObjectList(pageIndex, pageCount, where, z => z.Id, OrderingType.Descending);
        }


        public List<Order> GetOrderList(int lastId, int pageCount, int accountId, Order_Status? state = null)
        {
            var seh = new SenparcExpressionHelper<Order>();
            seh.ValueCompare
                .AndAlso(lastId > 0, z => z.Id < lastId)
                .AndAlso(true, z => z.AccountId == accountId);

            var where = seh.BuildWhereExpression();

            return GetObjectList(1, pageCount, where, z => z.Id, OrderingType.Descending);
        }

        public int GetVisibleOrderCount(int accountId)
        {
            return
                GetCount(
                    z =>
                        z.AccountId == accountId && z.Status != (int)Order_Status.已取消 &&
                        z.Status != (int)Order_Status.未支付);
        }

        ///  <summary>
        /// 二维码支付
        ///  </summary>
        ///  <param name="order"></param>
        ///  <param name="spbillCreateIp"></param>
        /// <returns></returns>
        public string TenPayV3QrCodeInfo(Order order, string spbillCreateIp)
        {
            var appId = SiteConfig.AppId; //AppId
            var mchId = SiteConfig.MCHID; //mchId
            var key = SiteConfig.MCHKEY; //key
            //var serviceKey = SiteConfig.SERVICEMCHKEY;
            //var serviceMchId = SiteConfig.SERVICEMCHID;
            //var serviceAppId = SiteConfig.SERVICEAPPID;
            var tenPayV3Notify = "{0}/TenPayResult".With(SiteConfig.DomainName);

            var tenPay = new TenPay(appId, mchId, key, tenPayV3Notify);

            var body = order.Description;
            var codeUrl = order.PrepayCodeUrl;
            //订单是否失效
            if (order.GetPayOrderTime == null ||
                DateTime.Compare(order.GetPayOrderTime.Value.AddHours(2), DateTime.Now) < 0)
            {
                try
                {
                    //获取微信预支付订单
                    string prepayId;

                    //判断订单模式
                    codeUrl = tenPay.GetCodeUrl(order.OrderNumber, body,
                        spbillCreateIp, order.TotalPrice, out prepayId);

                    //记录微信的订单号
                    order.PrepayId = prepayId;
                    order.PrepayCodeUrl = codeUrl;
                    //TODO:获取预支付订单时间【只有两个小时的有效期】
                    order.GetPayOrderTime = DateTime.Now;
                    SaveObject(order);
                }
                catch (Exception ex)
                {
                    LogUtility.Order.Error("支付失败：{0}".With(ex.Message), ex);
                    throw ex;
                }
            }
            return codeUrl;
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="ip"></param>
        /// <param name="payMoney"></param>
        /// <param name="price"></param>
        /// <param name="description"></param>
        /// <param name="orderType"></param>
        /// <param name="payType"></param>
        /// <returns></returns>
        public Order AddOrder(int activityId, string ip, decimal payMoney, decimal price, string description,
            Order_Type orderType, Order_PayType payType = Order_PayType.微信支付)
        {
            var order = new Order()
            {
                OrderNumber = this.GetAvalibleOrderNumber(),
                AddTime = DateTime.Now,
                CompleteTime = DateTime.Now,
                //AccountId = accountId,
                ActivityId = activityId,
                AddIp = ip,
                PayMoney = payMoney,
                Price = price,
                Description = description ?? "",
                Status = (int)Order_Status.未支付,
                PayType = (int)payType,
                TotalPrice = payMoney,
                OrderType = (int)orderType,
            };
            SaveObject(order);
            return order;
        }


        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <returns></returns>
        public string GetAvalibleOrderNumber()
        {
            string orderNumber = null;
            Order order = null;
            do
            {
                orderNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
                order = this.GetByOrderNumber(orderNumber);
            } while (order != null);

            return orderNumber;
        }

        public Order GetByOrderNumber(string orderNumber, string[] includes = null)
        {
            return this.GetObject(z => z.OrderNumber == orderNumber, includes);
        }

        /// <summary>
        /// 支付完成
        /// </summary>
        /// <param name="order"></param>
        public void OrderFinish(Order order)
        {
            var redPackageActivityService = ObjectFactory.GetInstance<APP_RedPackage_ActivityService>();
            var redPackageActivityLogService = ObjectFactory.GetInstance<APP_RedPackage_Activity_LogService>();
            var redPackageActivity = redPackageActivityService.GetObject(z => z.Id == order.ActivityId);
            redPackageActivity.TotalMoney += order.PayMoney;
            //剩余金额等于总金额减去当前已发出金额
            //var redPackageActivityLogList =
            //    redPackageActivityLogService.GetFullList(z => z.ActivityId == order.ActivityId, z => z.Id,
            //        OrderingType.Descending);

            //var usedMoney = redPackageActivityLogList.Sum(z => z.Money);
            //TODO:如果用过的大于当前的
            //if (usedMoney != redPackageActivity.TotalMoney - redPackageActivity.RemainingMoney)
            //{
            //    //TODO:当前余额==当前总额-已经发出去的总额
            //    //TODO:退还充值金额
            //    throw new Exception("当前活动账户资金异常！");
            //}
            //redPackageActivity.RemainingMoney = redPackageActivity.TotalMoney - usedMoney;
            redPackageActivity.RemainingMoney = redPackageActivity.TotalMoney - redPackageActivity.RemainingMoney;

            //TODO:是否需要保存AccountId
            order.Status = (int)Order_Status.已支付;
            order.CompleteTime = DateTime.Now;
            SaveObject(order);
        }

        public override void SaveObject(Order obj)
        {
            var isInsert = base.IsInsert(obj);
            base.SaveObject(obj);
            LogUtility.WebLogger.InfoFormat("Order{2}：{0}（ID：{1}）", obj.OrderNumber, obj.Id, isInsert ? "新增" : "编辑");
        }

        public override void DeleteObject(Order obj)
        {
            LogUtility.WebLogger.InfoFormat("Order被删除：{0}（ID：{1}）", obj.OrderNumber, obj.Id);
            base.DeleteObject(obj);
        }
    }
}
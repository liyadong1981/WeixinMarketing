/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：TenPayResultController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System.Web.Mvc;
using Senparc.Core.Cache;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Log;
using Senparc.Service;
using Senparc.Weixin.MP.TenPayLibV3;
using StructureMap;

namespace Senparc.Mvc.Controllers
{
    public class TenPayResultController : Controller//NOT BaseController
    {
        private object PayLock = new object();

        public ActionResult Index()
        {
            ResponseHandler resHandler = new ResponseHandler(null);
            resHandler.Init();
            resHandler.SetKey(SiteConfig.MCHKEY);

            if (!resHandler.IsTenpaySign())
            {
                return Content("签名错误！");
            }

            string return_code = resHandler.GetParameter("return_code");
            string return_msg = resHandler.GetParameter("return_msg");

            //即时到账
            if (return_code == "SUCCESS")
            {
                string result_code = resHandler.GetParameter("result_code");
                string err_code = resHandler.GetParameter("err_code");

                //验证请求是否从微信发过来（安全）
                if (!resHandler.IsTenpaySign() || return_code.ToUpper() != "SUCCESS")  //支付签名检验
                {
                    LogUtility.Order.ErrorFormat("订单支付失败：{0} / {1}", return_code, return_msg);
                    return Content("wrong");//错误的订单处理
                }
                else
                {
                    //直到这里，才能认为交易真正成功了，可以进行数据库操作，但是别忘了返回规定格式的消息！

                    var orderService = ObjectFactory.GetInstance<OrderService>();

                    string out_trade_no = resHandler.GetParameter("out_trade_no");

                    var order = orderService.GetObject(z => z.OrderNumber == out_trade_no);
                    //判断订单是否被处理过
                    if (order.PayType == (int)Order_PayType.微信支付 && order.Status == (int)Order_Status.已支付)
                    {
                        LogUtility.Order.InfoFormat("订单{0}已被处理", order.OrderNumber);
                        return Content("SUCCESS");
                    }
                    lock (PayLock)
                    {
                        //TODO：检查支付信息

                        var fullSystemConfigCache = ObjectFactory.GetInstance<IFullSystemConfigCache>();
                        var fullSystemConfig = fullSystemConfigCache.Data;

                        //订单支付成功处理
                        orderService.OrderFinish(order);
                        LogUtility.Order.InfoFormat("订单{0}支付成功，处理成功", order.OrderNumber);
                        return Content("SUCCESS");
                    }
                }
            }
            else
            {
                LogUtility.Order.ErrorFormat("订单支付失败：{0} / {1}", return_code, return_msg);
                return Content(return_msg);
            }

        }
    }
}

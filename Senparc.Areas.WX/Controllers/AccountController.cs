/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AccountController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System.Web.Mvc;
using Senparc.Mvc.Filter;


namespace Senparc.Areas.WX.Controllers
{
    [MenuFilter("User")]
    public class AccountController : BaseWXController
    {
//        private IOrderService _orderService;
//        private ICouponService _couponService;
//        private IShopService _shopService;
//        IAccountService _accountService;
//        public AccountController(IOrderService orderService, ICouponService couponService, IShopService shopService, IAccountService accountService)
//        {
//            _orderService = orderService;
//            _couponService = couponService;
//            _shopService = shopService;
//            _accountService = accountService;
//        }

        public ActionResult Index()
        {
//            int buyerOrderCount = _orderService.GetVisibleOrderCount(FullAccount.Id);
//            var state = (int) Coupon_State.未使用;
//            int coupon = _couponService.GetCount(z => z.AccountId == FullAccount.Id && z.State==state);
//            var vd = new Account_IndexVD()
//            {
//                BuyerOrderCount = buyerOrderCount,
//                Coupon = coupon,
//                Points = FullAccount.Points,
//                Shop = _shopService.GetObject(z=>z.Id == FullAccount.Id),
//            };
//
//            //我是卖家
//            if (vd.Shop!=null)
//            {
//                var account = _accountService.GetObject(z=>z.Id == FullAccount.Id);
//
//                var childrenAccountIds = account.ChildrenAccount.Select(z=>z.Id);
//                var monthTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
//                Expression<Func<Order,bool>> where = z => childrenAccountIds.Contains(z.AccountId) && z.IsPaid && z.PayTime!=null && z.PayTime > monthTime;
//                vd.Seller_OrderCount = _orderService.GetCount(where);
//                vd.Seller_OrderMoney = _orderService.GetSum(where,z=>z.TotalPrice);
//                var tobeShippedState = (int)Order_State.待发货;
//                vd.Seller_OrderToBeShipped = _orderService.GetCount(z => childrenAccountIds.Contains(z.AccountId) && z.IsPaid && z.PayTime != null && z.PayTime > monthTime && z.State == tobeShippedState);
//                var shippedState = (int)Order_State.待收货;
//                vd.Seller_OrderShipped = _orderService.GetCount(z => childrenAccountIds.Contains(z.AccountId) && z.IsPaid && z.PayTime != null && z.PayTime > monthTime && z.State == shippedState);
//            }

//            return View(vd);
            return View();
        }
    }
}

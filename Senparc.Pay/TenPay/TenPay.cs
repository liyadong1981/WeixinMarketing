/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：TenPay.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLib;
using RequestHandler = Senparc.Weixin.MP.TenPayLibV3.RequestHandler;
using Senparc.Log;
using Senparc.Weixin.MP.TenPayLibV3;

namespace Senparc.Pay.TenPay
{
    //微信支付JsApi需要的参数
    public class TenPayV3_JsApi
    {
        public string AppId { get; set; }
        public string TimeStamp { get; set; }
        public string NonceStr { get; set; }
        public string Package { get; set; }
        public string PaySign { get; set; }
    }

    public class TenPay
    {
        private static string AppId { get; set; }
        private static string MchId { get; set; }

        private static string Key { get; set; }
        private static string TenPayV3Notify { get; set; }

        /// <summary>
        /// 微信支付
        /// </summary>
        /// <param name="appId">公众号appId</param>
        /// <param name="mchId">商户号</param>
        /// <param name="key">商户私钥</param>
        /// <param name="tenPayV3Notify">微信支付回调地址</param>
        public TenPay(string appId, string mchId, string key, string tenPayV3Notify)
        {
            AppId = appId;
            MchId = mchId;
            Key = key;
            TenPayV3Notify = tenPayV3Notify;
        }
        /// <summary>
        /// 获取预支付订单
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="body">支付主体内容</param>
        /// <param name="spbillCreateIp">IP</param>
        /// <param name="openId"></param>
        /// <param name="price"></param>
        /// <param name="nonceStr"></param>
        /// <returns></returns>
        public string GetPrepayId(string orderId, string body, string spbillCreateIp, string openId, decimal price, string nonceStr = null)
        {
            //创建支付应答对象
            RequestHandler packageReqHandler = new RequestHandler(null);
            //初始化
            packageReqHandler.Init();

            nonceStr = string.IsNullOrEmpty(nonceStr) ? TenPayUtil.GetNoncestr() : nonceStr;

            //设置package订单参数
            packageReqHandler.SetParameter("appid", AppId);		  //公众账号ID
            packageReqHandler.SetParameter("mch_id", MchId);		//商户号
            packageReqHandler.SetParameter("nonce_str", nonceStr);   //随机字符串
            packageReqHandler.SetParameter("body", body);
            packageReqHandler.SetParameter("out_trade_no", orderId);		//商家订单号
            packageReqHandler.SetParameter("total_fee", (price * 100).ToString("0"));			        //商品金额,以分为单位(money * 100).ToString()
            packageReqHandler.SetParameter("spbill_create_ip", spbillCreateIp);   //用户的公网ip，不是商户服务器IP
            packageReqHandler.SetParameter("notify_url", TenPayV3Notify);		    //接收财付通通知的URL
            packageReqHandler.SetParameter("trade_type", "JSAPI");	                    //交易类型
            packageReqHandler.SetParameter("openid", openId);	                    //用户的openId

            string sign = packageReqHandler.CreateMd5Sign("key", Key);
            packageReqHandler.SetParameter("sign", sign);	                    //签名

            string data = packageReqHandler.ParseXML();

            var result = TenPayV3.Unifiedorder(data);
            var res = XDocument.Parse(result);

            if (res.Element("xml") == null)
            {
                throw new Exception("统一订单接口出错");
            }

            var prepayId = res.Element("xml").Element("prepay_id") == null ? null : res.Element("xml").Element("prepay_id").Value;

            if (string.IsNullOrEmpty(prepayId))
            {
                throw new Exception("统一订单接口出错，未获取到预支付订单号");
            }

            return prepayId;
        }

        /// <summary>
        /// 获取预支付订单
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="body">支付主体内容</param>
        /// <param name="spbillCreateIp">IP</param>
        /// <param name="openId"></param>
        /// <param name="price"></param>
        /// <param name="parentId">返回的预支付订单【2小时有效】</param>
        /// <param name="nonceStr"></param>
        /// <returns></returns>
        public string GetCodeUrl(string orderId, string body, string spbillCreateIp, decimal price, out string parentId,
            string nonceStr = null)
        {
            //创建支付应答对象
            RequestHandler packageReqHandler = new RequestHandler(null);
            //初始化
            packageReqHandler.Init();

            nonceStr = string.IsNullOrEmpty(nonceStr) ? TenPayUtil.GetNoncestr() : nonceStr;

            //设置package订单参数
            packageReqHandler.SetParameter("appid", AppId);		  //公众账号ID
            packageReqHandler.SetParameter("mch_id", MchId);		//商户号
            packageReqHandler.SetParameter("nonce_str", nonceStr); //随机字符串
            packageReqHandler.SetParameter("body", body);
            packageReqHandler.SetParameter("out_trade_no", orderId); //商家订单号
            packageReqHandler.SetParameter("total_fee", (price * 100).ToString("0")); //商品金额,以分为单位(money * 100).ToString()
            packageReqHandler.SetParameter("spbill_create_ip", spbillCreateIp); //用户的公网ip，不是商户服务器IP
            packageReqHandler.SetParameter("notify_url", TenPayV3Notify); //接收财付通通知的URL
            packageReqHandler.SetParameter("trade_type", "NATIVE"); //交易类型
                                                                    //packageReqHandler.SetParameter("openid", openId); //用户的openid
            string sign = packageReqHandler.CreateMd5Sign("key", Key);
            packageReqHandler.SetParameter("sign", sign); //签名

            string data = packageReqHandler.ParseXML();

            var result = TenPayV3.Unifiedorder(data);
            LogUtility.DebugLogger.Info(result);

            var res = XDocument.Parse(result);
            //LogUtility.Order.Error(string.Format("预支付订单返回XML :{0}", result));
            if (res.Element("xml") == null)
            {
                throw new Exception("统一订单接口出错");
            }
            var codeUrl = res.Element("xml").Element("code_url") == null
               ? null
               : res.Element("xml").Element("code_url").Value;
            parentId = res.Element("xml").Element("prepay_id") == null
              ? null
              : res.Element("xml").Element("prepay_id").Value;
            if (string.IsNullOrEmpty(codeUrl))
            {
                throw new Exception("统一订单接口出错，未获取到预支付订单号");
            }
            return codeUrl;

        }
        /// <summary>
        /// 获取微信支付必要的参数
        /// </summary>
        /// <param name="prepayId">微信预支付订单号</param>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="nonceStr">随机数</param>
        /// <returns></returns>
        public TenPayV3_JsApi TenPayV3Info(string prepayId, string timeStamp = null, string nonceStr = null)
        {
            string paySign = "";

            nonceStr = string.IsNullOrEmpty(nonceStr) ? TenPayUtil.GetNoncestr() : nonceStr;
            timeStamp = string.IsNullOrEmpty(timeStamp) ? TenPayUtil.GetTimestamp() : timeStamp;

            //设置支付参数
            RequestHandler paySignReqHandler = new RequestHandler(null);
            paySignReqHandler.SetParameter("appId", AppId);
            paySignReqHandler.SetParameter("nonceStr", nonceStr);
            paySignReqHandler.SetParameter("package", string.Format("prepay_id={0}", prepayId));
            paySignReqHandler.SetParameter("signType", "MD5");
            paySignReqHandler.SetParameter("timeStamp", timeStamp);
            paySign = paySignReqHandler.CreateMd5Sign("key", Key);
            TenPayV3_JsApi tenPayV3JsApi = new TenPayV3_JsApi()
            {
                AppId = AppId,
                NonceStr = nonceStr,
                TimeStamp = timeStamp,
                Package = string.Format("prepay_id={0}", prepayId),
                PaySign = paySign
            };
            return tenPayV3JsApi;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        /// <summary>
        /// 提现转账
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="money"></param>
        /// <param name="openId"></param>
        /// <param name="checkName">【NO_CHECK：不校验真实姓名 FORCE_CHECK：强校验真实姓 OPTION_CHECK：针对已实名认证的用户才校验真实姓名】</param>
        /// <param name="reUserName"></param>
        /// <param name="spbillCreateIp"></param>
        /// <param name="desc"></param>
        /// <param name="nonceStr"></param>
        public void WithdrawMoney(string orderId, decimal money, string openId, string checkName, string reUserName,
            string spbillCreateIp, string desc,
            string nonceStr = null)
        {
            //创建支付应答对象
            RequestHandler packageReqHandler = new RequestHandler(null);
            //初始化
            packageReqHandler.Init();

            nonceStr = string.IsNullOrEmpty(nonceStr) ? TenPayUtil.GetNoncestr() : nonceStr;

            //设置package订单参数
            packageReqHandler.SetParameter("mch_appid", AppId); //公众账号ID
            packageReqHandler.SetParameter("mchid", MchId); //商户号
            packageReqHandler.SetParameter("nonce_str", nonceStr); //随机字符串
            packageReqHandler.SetParameter("desc", desc); //企业付款描述信息
            packageReqHandler.SetParameter("check_name", checkName); //校验用户姓名选项
            packageReqHandler.SetParameter("re_user_name", reUserName); //收款用户姓名 
            packageReqHandler.SetParameter("partner_trade_no", orderId); //商户订单号
            packageReqHandler.SetParameter("amount", (money * 100).ToString("0")); //转账金额,以分为单位(money * 100).ToString()
            packageReqHandler.SetParameter("spbill_create_ip", spbillCreateIp); //用户的公网ip，不是商户服务器IP
            packageReqHandler.SetParameter("openid", openId); //用户的openId
            string sign = packageReqHandler.CreateMd5Sign("key", Key);
            packageReqHandler.SetParameter("sign", sign); //签名
            string data = packageReqHandler.ParseXML();

            #region transfers()

            //发企业支付接口地址
            string url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";

            //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
            //string cert = @"D:\apiclient_cert.p12";
            //私钥（在安装证书时设置）
            string password = MchId;
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(CheckValidationResult);
            //调用证书
            //X509Certificate2 cer = new X509Certificate2(cert, password,
            //    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);


            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                       X509FindType.FindByThumbprint, "9595BAC7475049BA7CB06ACD6CDC792BADF40217", false);

            X509Certificate2 cert = null;
            if (certCollection.Count > 0)
            {
                cert = certCollection[0];
            }

            #region 发起post请求

            HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webrequest.ClientCertificates.Add(cert);
            webrequest.Method = "post";

            byte[] postdatabyte = Encoding.UTF8.GetBytes(data);
            webrequest.ContentLength = postdatabyte.Length;
            Stream stream;
            stream = webrequest.GetRequestStream();
            stream.Write(postdatabyte, 0, postdatabyte.Length);
            stream.Close();

            HttpWebResponse httpWebResponse = (HttpWebResponse)webrequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string result = streamReader.ReadToEnd();

            #endregion

            #endregion

            var res = XDocument.Parse(result);

            if (res.Element("xml") == null)
            {
                throw new Exception("转账订单接口出错");
            }

            var returnCode = res.Element("xml").Element("return_code") == null
                ? null
                : res.Element("xml").Element("return_code").Value;
            var resultCode = res.Element("xml").Element("result_code") == null
                ? null
                : res.Element("xml").Element("result_code").Value;
            if (string.IsNullOrEmpty(returnCode))
            {
                throw new Exception("转账订单接口出错，未获取到返回状态码");
            }
            if (returnCode == "FAIL" || resultCode == "FAIL")
            {
                var returnMsg = res.Element("xml").Element("return_msg").ToString();
                throw new Exception(returnMsg);
            }
        }
    }
}
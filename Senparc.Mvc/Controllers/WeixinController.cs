using System;
using System.IO;
using System.Web.Mvc;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Service;
using Senparc.Weixin;
using Senparc.Core.Extensions;
using Senparc.Weixin.MessageHandlers;
using Senparc.Weixin.QY;
using Senparc.Weixin.QY.Entities;
using Senparc.Weixin.QY.MessageHandlers;
using Senparc.Weixin.MP.MvcExtension;

namespace Senparc.Mvc.Controllers
{
    /// <summary>
    /// 企业号接收微信推送信息
    /// </summary>
    public class WeixinController : Controller
    {
        private IEncryptionService _encryptionService;
        public WeixinController(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信企业后台应用的“修改配置”的Url填写如：http://weixin.senparc.com/qy
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string key/*即加密key*/, string msg_signature = "", string timestamp = "", string nonce = "", string echostr = "")
        {
            //根据加密的Key获取QyApp信息
            var qyApp = _encryptionService.CheckDecodedQyAppKey(key, SiteConfig.WEIXIN_QY_KEY_KEY);
            if (qyApp == null)
            {
                return null;
            }

            //return Content(echostr); //返回随机字符串则表示验证通过
            var verifyUrl = Senparc.Weixin.QY.Signature.VerifyURL(qyApp.Token, qyApp.EncodingAESKey, qyApp.CorpId, msg_signature, timestamp, nonce,
                echostr);
            if (verifyUrl != null)
            {
                return Content(verifyUrl); //返回解密后的随机字符串则表示验证通过
            }
            else
            {
                return Content("如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。" + key);
            }
        }
        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult PostPost(string key, PostModel postModel)
        {
            //根据加密的Key获取QyApp信息
            var qyApp = _encryptionService.CheckDecodedQyAppKey(key, SiteConfig.WEIXIN_QY_KEY_KEY);
            if (qyApp == null)
            {
                return null;
            }

            postModel.Token = qyApp.Token;
            postModel.EncodingAESKey = qyApp.EncodingAESKey;
            postModel.CorpId = qyApp.CorpId;

            var appKind = (AppKind)qyApp.Id;

            var dir = Senparc.Core.Utility.Server.GetMapPath("~/App_Data/QyAppLog/{0}/{1}".With(appKind.ToString(), DateTime.Now.ToString("yyyy-MM-dd")));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                IMessageHandler<IRequestMessageBase, IResponseMessageBase> messageHandler = AppMessageHandlerFactory.GetMessageHandler(appKind, Request.InputStream, postModel, qyApp);

                var doc = messageHandler.RequestDocument;
                doc.Save(Path.Combine(dir,
                    "{0}_Request_{1}.txt".With(DateTime.Now.Ticks, messageHandler.RequestMessage.FromUserName)));
                //测试时可开启，帮助跟踪数据

                messageHandler.Execute(); //执行微信操作

                var responseDoc = messageHandler.ResponseDocument;
                responseDoc.Save(Path.Combine(dir,
                    "{0}_Response_{1}.txt".With(DateTime.Now.Ticks, messageHandler.ResponseMessage.ToUserName)));

                //测试时可开启，帮助跟踪数据

                //return Content(responseDoc.ToString());
                return new FixWeixinBugWeixinResult(messageHandler);
                //return new WeixinResult(messageHandler);
            }
            catch (Exception ex)
            {
                //LogUtility.WeixinLogger.Error(ex.Message, ex);
                TextWriter tw =
                    new StreamWriter(Path.Combine(dir, "Error_{0}.txt".With(DateTime.Now.Ticks + "_" + ex.Message)));
                tw.WriteLine(ex.Message);
                tw.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    tw.WriteLine(ex.InnerException.Message);
                }

                tw.Flush();
                tw.Close();
                return Content("");
            }
        }

    }
}

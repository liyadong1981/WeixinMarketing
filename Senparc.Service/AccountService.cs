/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AccountService.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Security;
using Senparc.Core.Cache;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Core.Utility;
using Senparc.Repository;
using Senparc.Log;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using StructureMap;

namespace Senparc.Service
{
    public interface IAccountService : IBaseClientService<Account>
    {
        /// <summary>
        /// 检验用户名是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool CheckUserNameExisted(long id, string userName);
        bool CheckEmailExisted(long id, string email);
        bool CheckPhoneExisted(long id, string phone);
        Account GetAccount(FullAccount fullAccount);
        /// <summary>
        /// 根据用户名获取账号信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Account GetAccount(string userName);
        Account GetAccount(string userName, string password);
        Account GetAccountByWeixinOpenId(string openId);
        string GetPassword(string password, string salt, bool isMD5Password);
        void Login(string userName, bool rememberMe, IEnumerable<string> roles, bool recordLoginInfo);
        Account TryLogin(string userNameOrEmailOrPhone, string password, bool rememberMe, bool recordLoginInfo);
        void Logout();
        bool CheckPassword(string userName, string password);
        Account CreateAccount(string userName, string email, string phone, string password, string weixinOpenId);

        /// <summary>
        /// 未验证手机号的用户通过验证记录手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="accountId"></param>
        void PhoneCheckPass(string phone, int accountId);

        /// <summary>
        /// 根据OAuth信息创建Account
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parentAccountId"></param>
        /// <returns></returns>
        Account CreateAccountByUserInfo(OAuthUserInfo userInfo, int parentAccountId = 0);

        /// <summary>
        /// 更新Account的微信账户信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="account"></param>
        void UpdateAccountByUserInfo(OAuthUserInfo userInfo, Account account);
        /// <summary>
        /// 根据UserInfoJson创建或更新Account
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        Account CreateOrUpdateByUserInfo(UserInfoJson userInfo, Account account = null);
        /// <summary>
        /// 获取头像路径
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetAvatarPath(int id);
    }

    public class AccountService : BaseClientService<Account>, IAccountService
    {
        public AccountService(IAccountRepository accountRepo)
            : base(accountRepo)
        {
        }

        public bool CheckUserNameExisted(long id, string userName)
        {
            return
                this.GetObject(
                    z => z.Id != id && z.UserName.Equals(userName.Trim(), StringComparison.CurrentCultureIgnoreCase)) !=
                null;
        }

        public bool CheckEmailExisted(long id, string email)
        {
            return
                this.GetObject(
                    z => z.Id != id && z.Email.Equals(email.Trim(), StringComparison.CurrentCultureIgnoreCase)) != null;
        }

        public bool CheckPhoneExisted(long id, string phone)
        {
            return
                this.GetObject(
                    z => z.Id != id && z.Phone.Equals(phone.Trim()) && z.PhoneChecked) !=
                null;
        }

        public Account GetAccount(FullAccount fullAccount)
        {
            if (fullAccount == null)
            {
                return null;
            }
            return GetAccount(fullAccount.UserName);
        }

        public Account GetAccount(string userName)
        {
            userName = userName.Trim();
            return this.GetObject(z => z.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                                       || z.Email.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
        }

        public Account GetAccount(string userName, string password)
        {
            userName = userName.Trim();
            Account account =
                this.GetObject(z => z.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                                    || z.Email.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                                    || z.Phone.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
            if (account == null)
            {
                return null;
            }
            var codedPassword = this.GetPassword(password, account.PasswordSalt, false);
            return account.Password == codedPassword ? account : null;
        }

        public Account GetAccountByWeixinOpenId(string openId)
        {
            return GetObject(z => z.WeixinOpenId == openId);
        }

        public string GetPassword(string password, string salt, bool isMD5Password)
        {
            string md5 = password.ToUpper().Replace("-", "");
            if (!isMD5Password)
            {
                md5 = MD5.GetMD5Code(password, "").Replace("-", ""); //原始MD5
            }
            return MD5.GetMD5Code(md5, salt).Replace("-", ""); //再加密
        }

        //public void ForceLogout(string userName)
        //{
        //    var fullAccountCache = StructureMap.ObjectFactory.GetInstance<IFullAccountCache>();
        //    var fullAccount = fullAccountCache.GetObject(userName);
        //    if (fullAccount==null)
        //    {
        //        return;
        //    }
        //    fullAccount.ForceLogout = true;
        //    fullAccount.LastActiveTime = DateTime.MinValue;
        //}

        public virtual void Logout()
        {
            try
            {
                FormsAuthentication.SignOut(); //退出网站登录
                //继续删除其他登陆信息
            }
            catch (Exception ex)
            {
                Log.LogUtility.Account.Error("退出登录失败。", ex);
            }
        }

        public virtual void Login(string userName, bool rememberMe, IEnumerable<string> roles, bool recordLoginInfo)
        {
            FormsAuthentication.SetAuthCookie(userName, rememberMe);
            if (recordLoginInfo)
            {
                var account = this.GetAccount(userName);
                if (account != null)
                {
                    account.LastLoginTime = account.ThisLoginTime;
                    account.LastLoginIP = account.ThisLoginIP;
                    account.ThisLoginTime = DateTime.Now;
                    account.ThisLoginIP = HttpContext.Current != null ? HttpContext.Current.Request.UserHostName : "";
                    this.SaveObject(account); //保存Account信息，同时会清除FullAccount信息，顺便保证“强制退出”等参数失效。
                }
            }
        }


        public bool CheckPassword(string userName, string password)
        {
            var account = this.GetObject(z => z.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (account == null)
            {
                return false;
            }
            var codedPassword = this.GetPassword(password, account.PasswordSalt, false);
            return account.Password == codedPassword;
        }

        public Account TryLogin(string userNameOrEmailOrPhone, string password, bool rememberMe, bool recordLoginInfo)
        {
            Account account = this.GetAccount(userNameOrEmailOrPhone, password);
            if (account != null)
            {
                this.Login(account.UserName, rememberMe, null, recordLoginInfo);
                return account;
            }
            else
            {
                return null;
            }
        }

        public Account CreateAccount(string userName, string email, string phone, string password, string weixinOpenId)
        {
            var passwordSalt = DateTime.Now.Ticks.ToString();
            var account = new Account()
            {
                UserName = userName,
                WeixinOpenId = weixinOpenId,
                Email = email,
                Password = this.GetPassword(password, passwordSalt, false),
                PasswordSalt = passwordSalt,
                AddTime = DateTime.Now,
                Sex = (int)Sex.未设置,
                ThisLoginTime = DateTime.Now,
                LastLoginTime = DateTime.Now,
                Phone = phone,
                Type = (int)Account_Type.普通用户,
            };

            this.SaveObject(account);
            return account;
        }

        /// <summary>
        /// 自动生成新的用户名
        /// </summary>
        /// <returns></returns>
        private string GetNewUserName()
        {
            string userName;
            Account account;

            do
            {
                userName = "azure_senparc_{0}".With(Guid.NewGuid().ToString("n").Substring(0, 8));
                account = this.GetAccount(userName);
            } while (account != null);

            return userName;
        }

        public Account CreateAccountByUserInfo(OAuthUserInfo userInfo, int parentAccountId = 0)
        {
            var time = DateTime.Now;
            Account account = null;
            string userName = GetNewUserName();

            account = CreateAccount(userName, "", "", "", userInfo.openid);
            account.Sex = (byte)userInfo.sex;
            account.HeadImgUrl = userInfo.headimgurl;
            account.NickName = userInfo.nickname;
            //account.Country = userInfo.country;
            account.Province = userInfo.province;
            account.City = userInfo.city;

            var fileName = @"/Upload/Account/headimgurl.{0}.jpg".With(DateTime.Now.Ticks + Guid.NewGuid().ToString("n").Substring(0, 8));

            //下载图片
            DownLoadPic(userInfo.headimgurl, fileName);
            account.PicUrl = fileName;

            SaveObject(account);
            return account;
        }


        public void UpdateAccountByUserInfo(OAuthUserInfo userInfo, Account account)
        {
            LogUtility.Account.InfoFormat("用户【{0}】微信信息更新：{0},{1},{2}", userInfo.openid, userInfo.headimgurl, userInfo.nickname);

            //删除图片
            if (!account.PicUrl.IsNullOrEmpty())
            {
                File.Delete(Server.GetMapPath("~" + account.PicUrl));
            }

            //account.WeixinOpenId = userInfo.openid;
            account.Sex = (byte)userInfo.sex;
            account.HeadImgUrl = userInfo.headimgurl;
            account.NickName = userInfo.nickname;
            account.Province = userInfo.province;
            account.City = userInfo.city;
            var fileName = @"/Upload/Account/headimgurl.{0}.jpg".With(DateTime.Now.Ticks + Guid.NewGuid().ToString("n").Substring(0, 8));

            //下载图片
            DownLoadPic(userInfo.headimgurl, fileName);

            account.PicUrl = fileName;
            this.SaveObject(account);
        }

        public Account CreateOrUpdateByUserInfo(UserInfoJson userInfo, Account account = null)
        {
            var time = DateTime.Now;
            string userName = GetNewUserName();

            if (account == null)
            {
                account = CreateAccount(userName, "", "", "", userInfo.openid);
            }

            account.NickName = userInfo.nickname;

            var defaultHeadimgUrl = "/{0}/Content/Images/userinfonopic.png".With(SiteConfig.DomainName);

            if (userInfo.headimgurl.IsNullOrEmpty())
            {
                account.HeadImgUrl = defaultHeadimgUrl;
                account.PicUrl = defaultHeadimgUrl;
            }
            else
            {
                account.HeadImgUrl = userInfo.headimgurl;

                var fileName = @"/Upload/Account/headimgurl.{0}.jpg".With(DateTime.Now.Ticks + Guid.NewGuid().ToString("n").Substring(0, 8));

                //下载图片
                DownLoadPic(userInfo.headimgurl, fileName);

                account.PicUrl = fileName;
            }

            SaveObject(account);
            return account;
        }

        public Account CreateAccountOAuthUserInfo(OAuthUserInfo oAuthUserInfo)
        {
            Account account = null;
            string userName = GetNewUserName();

            account = CreateAccount(userName, "", "", "", oAuthUserInfo.openid);

            var defaultHeadimgUrl = "/{0}/Content/Images/userinfonopic.png".With(SiteConfig.DomainName);

            if (oAuthUserInfo.headimgurl.IsNullOrEmpty())
            {
                account.HeadImgUrl = defaultHeadimgUrl;
                account.PicUrl = defaultHeadimgUrl;
            }
            else
            {
                account.HeadImgUrl = oAuthUserInfo.headimgurl;

                var fileName = @"/Upload/Account/headimgurl.{0}.jpg".With(DateTime.Now.Ticks + Guid.NewGuid().ToString("n").Substring(0, 8));

                //下载图片
                DownLoadPic(oAuthUserInfo.headimgurl, fileName);

                account.PicUrl = fileName;
            }

            SaveObject(account);
            return account;
        }

        /// <summary>
        /// 下载图片到指定文件
        /// </summary>
        /// <param name="picUrl"></param>
        /// <param name="fileName"></param>
        private void DownLoadPic(string picUrl, string fileName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Get.Download(picUrl, stream);

                using (var fs = new FileStream(Server.GetMapPath("~" + fileName), FileMode.CreateNew))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fs);
                    fs.Flush();
                }
            }
        }

        public void PhoneCheckPass(string phone, int accountId)
        {
            var account = GetObject(accountId);

            account.Phone = phone;
            account.PhoneChecked = true;

            SaveObject(account);
        }

        public string GetAvatarPath(int id)
        {
            if (id <= 0)
            {
                return SiteConfig.DEFAULT_AVATAR;
            }

            var fullAccountCache = ObjectFactory.GetInstance<IFullAccountCache>();
            var fullAccount = fullAccountCache.GetFullAccount(id);
            if (fullAccount != null)
            {
                return fullAccount.PicUrl;
            }
            else
            {
                return SiteConfig.DEFAULT_AVATAR;
            }

            ////if (id >= SiteConfig.MIN_WEIXINUSERINFO_ID)
            ////{
            ////    var weixinUserInfo = GetObject(id);
            ////    if (weixinUserInfo == null)
            ////    {
            ////        return null;
            ////    }
            ////    //通过高级接口获得返回http地址
            ////    return weixinUserInfo.HeadImgUrl;
            ////}
            ////else
            ////{
            //    var catalog = id.ToString().Substring(1, 3);//按照第2位数字分类到文件夹
            //    var dir = Server.GetMapPath("~/Data/WeixinAvatar/" + catalog);
            //    //这里每次检查就先创建（虽然分离的不好）
            //    if (!Directory.Exists(dir))
            //    {
            //        Directory.CreateDirectory(dir);
            //    }
            //    var path = Path.Combine(dir, id + ".jpg");
            //    return path;
            ////}
        }

        public override void SaveObject(Account obj)
        {
            var isInsert = base.IsInsert(obj);
            base.SaveObject(obj);
            LogUtility.WebLogger.InfoFormat("Account{2}：{0}（ID：{1}）", obj.UserName, obj.Id, isInsert ? "新增" : "编辑");

            //清除缓存
            var fullAccountCache = StructureMap.ObjectFactory.GetInstance<IFullAccountCache>();
            fullAccountCache.RemoveObject(obj.UserName);
        }


        public override void DeleteObject(Account obj)
        {
            if (HttpContext.Current == null
                || !HttpContext.Current.User.Identity.IsAuthenticated
                || HttpContext.Current.User.Identity.Name.ToUpper() != "ADMIN")
            {
                LogUtility.SystemLogger.WarnFormat("尝试删除Account资料失败！IP：{0} / {1}", HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserHostName);

                //TODO:类似警告可以发送提示信息

                throw new Exception("您的权限不够！此次操作所有信息已被记录！");
            }

            base.DeleteObject(obj);
            LogUtility.WebLogger.InfoFormat("Account被删除：{0}（ID：{1}）", obj.UserName, obj.Id);


            //清除缓存
            var fullAccountCache = StructureMap.ObjectFactory.GetInstance<IFullAccountCache>();
            fullAccountCache.RemoveObject(obj.UserName);
        }
    }
}


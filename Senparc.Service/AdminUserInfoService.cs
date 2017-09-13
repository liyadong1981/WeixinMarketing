/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AdminUserInfoService.cs

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
using System.Web;
using System.Web.Security;
using Senparc.Core.Enums;
using Senparc.Core.Models;
using Senparc.Core.Utility;
using Senparc.Repository;
using Senparc.Log;
using Senparc.Core.Cache;

namespace Senparc.Service
{
    public interface IAdminUserInfoService : IBaseClientService<AdminUserInfo>
    {
        /// <summary>
        /// 检验用户名是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool CheckUserNameExisted(long id, string userName);
        /// <summary>
        /// 根据用户名获取账号信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        AdminUserInfo GetUserInfo(string userName);
        AdminUserInfo GetUserInfo(string userName, string password);
        string GetPassword(string password, string salt, bool isMD5Password);
        void Login(string userName, bool rememberMe);
        AdminUserInfo TryLogin(string userNameOrEmail, string password, bool rememberMe);
        void Logout();
        bool CheckPassword(string userName, string password);

        /// <summary>
        /// 根据Id获取AdminUserInfo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        AdminUserInfo GetAdminUserInfo(int id, string[] includes = null);
    }

    public class AdminUserInfoService : BaseClientService<AdminUserInfo>, IAdminUserInfoService
    {
        public AdminUserInfoService(IAdminUserInfoRepository adminUserInfoRepo)
            : base(adminUserInfoRepo)
        {

        }


        public bool CheckUserNameExisted(long id, string userName)
        {
            return this.GetObject(z => z.Id != id && z.UserName.Equals(userName.Trim(), StringComparison.CurrentCultureIgnoreCase)) != null;
        }

        public AdminUserInfo GetUserInfo(string userName)
        {
            return this.GetObject(z => z.UserName.Equals(userName.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        public AdminUserInfo GetUserInfo(string userName, string password)
        {
            AdminUserInfo userInfo = this.GetObject(z => z.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase), null);
            if (userInfo == null)
            {
                return null;
            }
            var codedPassword = this.GetPassword(password, userInfo.PasswordSalt, false);
            return userInfo.Password == codedPassword ? userInfo : null;
        }

        public string GetPassword(string password, string salt, bool isMD5Password)
        {
            string md5 = password.ToUpper().Replace("-", "");
            if (!isMD5Password)
            {
                md5 = MD5.GetMD5Code(password, "").Replace("-", "");//原始MD5
            }
            return MD5.GetMD5Code(md5, salt).Replace("-", "");//再加密
        }

        public virtual void Logout()
        {
            try
            {
                FormsAuthentication.SignOut();//退出网站登录
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Session.Remove("AdminLogin");//退出管理员状态
                }
            }
            catch (Exception ex)
            {
                Log.LogUtility.AdminUserInfo.Error("退出登录失败。", ex);
            }
        }

        public virtual void Login(string userName, bool rememberMe)
        {
            FormsAuthentication.SetAuthCookie(userName, rememberMe);
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["AdminLogin"] = userName;
            }
        }


        public bool CheckPassword(string userName, string password)
        {
            var userInfo = this.GetObject(z => z.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (userInfo == null)
            {
                return false;
            }
            var codedPassword = this.GetPassword(password, userInfo.PasswordSalt, false);
            return userInfo.Password == codedPassword;
        }

        public AdminUserInfo TryLogin(string userNameOrEmail, string password, bool rememberMe)
        {
            AdminUserInfo userInfo = this.GetUserInfo(userNameOrEmail, password);
            if (userInfo != null)
            {
                this.Login(userInfo.UserName, rememberMe);
                return userInfo;
            }
            else
            {
                return null;
            }
        }

        public AdminUserInfo GetAdminUserInfo(int id, string[] includes = null)
        {
            return GetObject(z => z.Id == id, includes: includes);
        }

        public override void SaveObject(AdminUserInfo obj)
        {
            var isInsert = base.IsInsert(obj);

            //保存上次/本次登录时间/IP
            //if (!isInsert)
            //{
            //    obj.LastLoginTime = DateTime.UtcNow;
            //    obj.LastLoginIP = HttpContext.Current.Request.UserHostAddress;
            //    obj.ThisLoginTime = DateTime.Now;
            //    obj.ThisLoginIP = HttpContext.Current.Request.UserHostAddress;
            //}

            base.SaveObject(obj);
            var userName = obj.UserName;
            LogUtility.WebLogger.InfoFormat("AdminUserInfo{2}：{0}（ID：{1}）", obj.UserName, obj.Id, isInsert ? "新增" : "编辑");
            //更新缓存
            var fullAdminUserInfoCache = StructureMap.ObjectFactory.GetInstance<IFullAdminUserInfoCache>();
            fullAdminUserInfoCache.RemoveObject(userName);
        }

        public override void DeleteObject(AdminUserInfo obj)
        {
            LogUtility.WebLogger.InfoFormat("AdminUserInfo被删除：{0}（ID：{1}）", obj.UserName, obj.Id);
            var userName = obj.UserName;
            base.DeleteObject(obj);
            //更新缓存
            var fullAdminUserInfoCache = StructureMap.ObjectFactory.GetInstance<IFullAdminUserInfoCache>();
            fullAdminUserInfoCache.RemoveObject(userName);
        }
    }
}


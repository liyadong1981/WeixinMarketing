/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：ExtensionEntity.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Senparc.Core.Enums;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using Senparc.Core.Config;
using Senparc.Core.Cache;
using Senparc.Log;
using Senparc.Core.Extensions;
using System.Web;
using System.Reflection;
using Senparc.Core.Models.WorkFlowModules;
using Senparc.Core.Utility;
using Senparc.Utility;
using StructureMap.Pipeline;
using WURFL;
using WURFL.Request;

namespace Senparc.Core.Models
{
    #region 全局

    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T> where T : class/*,new()*/
    {
        public PagedList(List<T> list, int pageIndex, int pageCount, int totalCount)
            : this(list, pageIndex, pageCount, totalCount, null)
        {
        }

        public PagedList(List<T> list, int pageIndex, int pageCount, int totalCount, int? skipCount = null)
        {
            this.AddRange(list);
            PageIndex = pageIndex;
            PageCount = pageCount;
            TotalCount = totalCount < 0 ? list.Count : totalCount;
            SkipCount = skipCount ?? Senparc.Core.Utility.Extensions.GetSkipRecord(pageIndex, pageCount);
        }

        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
        public int SkipCount { get; set; }
        public int TotalPageNumber
        {
            get
            {
                return Convert.ToInt32((TotalCount - 1) / PageCount) + 1;
            }
        }
    }

    /// <summary>
    /// 网页Meta标签集合
    /// </summary>
    public class MetaCollection : Dictionary<MetaType, string>
    {
        //new public string this[MetaType metaType]
        //{
        //    get
        //    {
        //        if (!this.ContainsKey(metaType))
        //        {
        //            this.Add(metaType, null);
        //        }
        //        return this[metaType];
        //    }
        //    set { this[metaType] = value; }
        //}
    }


    /// <summary>
    /// 系统配置文件
    /// </summary>
    public class SenparcConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string DataBase { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionStringFull { get; set; }
        public string ApplicationPath { get; set; }
    }


    /// <summary>
    /// 全局提示消息
    /// </summary>
    [Serializable]
    public class Messager
    {
        public MessageType MessageType { get; set; }
        public string MessageText { get; set; }
        public bool ShowClose { get; set; }
        public Messager(MessageType messageType, string messageText, bool showClose = true)
        {
            MessageType = messageType; MessageText = messageText;
            ShowClose = showClose;
        }
    }
    /// <summary>
    /// 日志
    /// </summary>
    public class WebLog
    {
        public DateTime DateTime { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string ThreadName { get; set; }
        public int PageIndex { get; set; }
        public int Line { get; set; }
    }

    public class AccountAuth
    {
        public DateTime LoginTime { get; set; }
        public DateTime LastActiveTime { get; set; }

    }

    #endregion

    #region 数据库实体扩展

    #region FullEntity相关

    public interface IBaseFullEntity<in TEntity>
    {
        void CreateEntity(TEntity entity);
    }


    [Serializable]
    public abstract class BaseFullEntity<TEntity> : IBaseFullEntity<TEntity>
          where TEntity : class, new()
    {
        public abstract string Key { get; }
        public DateTime CacheTime { get; set; }

        public BaseFullEntity()
        {
        }

        public virtual void CreateEntity(TEntity entity)
        {
            FullEntityCache.SetFullEntityCache(this, entity);
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T">对象类型（FullT）</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">实体实例</param>
        /// <returns></returns>
        public static T CreateEntity<T>(TEntity entity)
            where T : BaseFullEntity<TEntity>, new()
        {
            T obj = new T();
            obj.CreateEntity(entity);
            return obj;
        }

        /// <summary>
        /// 创建对象列表
        /// </summary>
        /// <typeparam name="T">对象类型（FullT）</typeparam>
        /// <typeparam name="TEntity">试题类型</typeparam>
        /// <param name="entityList">实体列表</param>
        /// <returns></returns>
        public static List<T> CreateList<T, FullT>(IEnumerable<TEntity> entityList)
            where T : BaseFullEntity<TEntity>, new()
        {
            var result = new List<T>();
            foreach (var item in entityList)
            {
                T obj = BaseFullEntity<TEntity>.CreateEntity<T>(item);
                result.Add(obj);
            }
            return result;
        }
    }

    #endregion

    [Serializable]
    public class FullSystemConfig : BaseFullEntity<SystemConfig>
    {
        public override string Key
        {
            get { return "FullSystemConfig." + SystemName; }
        }

        [AutoSetCache]
        public string SystemName { get; set; }
        [AutoSetCache]
        public string MchId { get; set; }
        [AutoSetCache]
        public string MchKey { get; set; }
        [AutoSetCache]
        public string TenPayAppId { get; set; }
    }

    [Serializable]
    public class FullAdminUserInfo : BaseFullEntity<AdminUserInfo>
    {
        public override string Key
        {
            get { return UserName.ToString(); }
        }

        [AutoSetCache]
        public int Id { get; set; }
        [AutoSetCache]
        public string UserName { get; set; }
        [AutoSetCache]
        public string Note { get; set; }
        [AutoSetCache]
        public DateTime ThisLoginTime { get; set; }
        [AutoSetCache]
        public string ThisLoginIP { get; set; }
        [AutoSetCache]
        public DateTime LastLoginTime { get; set; }
        [AutoSetCache]
        public string LastLoginIP { get; set; }
        [AutoSetCache]
        public DateTime AddTime { get; set; }

        public override void CreateEntity(AdminUserInfo entity)
        {
            base.CreateEntity(entity);
        }
    }

    [Serializable]
    public class FullAccount : BaseFullEntity<Account>
    {
        public override string Key
        {
            get { return UserName.ToString(); }
        }

        [AutoSetCache]
        public int Id { get; set; }
        public string UserName { get; set; }
        [AutoSetCache]
        public string Password { get; set; }
        [AutoSetCache]
        public string PasswordSalt { get; set; }
        [AutoSetCache]
        public string NickName { get; set; }
        [AutoSetCache]
        public string RealName { get; set; }
        [AutoSetCache]
        public string Tel { get; set; }
        [AutoSetCache]
        public string Email { get; set; }
        [AutoSetCache]
        public bool EmailChecked { get; set; }
        [AutoSetCache]
        public string Phone { get; set; }
        [AutoSetCache]
        public bool PhoneChecked { get; set; }
        [AutoSetCache]
        public string WeixinOpenId { get; set; }
        [AutoSetCache]
        public string PicUrl { get; set; }
        [AutoSetCache]
        public string HeadImgUrl { get; set; }
        [AutoSetCache]
        public byte Sex { get; set; }
        [AutoSetCache]
        public string QQ { get; set; }
        [AutoSetCache]
        public string Country { get; set; }
        [AutoSetCache]
        public string Province { get; set; }
        [AutoSetCache]
        public string City { get; set; }
        [AutoSetCache]
        public string District { get; set; }
        [AutoSetCache]
        public string Address { get; set; }
        [AutoSetCache]
        public string Note { get; set; }
        [AutoSetCache]
        public int Type { get; set; }
        [AutoSetCache]
        public System.DateTime ThisLoginTime { get; set; }
        [AutoSetCache]
        public string ThisLoginIP { get; set; }
        [AutoSetCache]
        public System.DateTime LastLoginTime { get; set; }
        [AutoSetCache]
        public string LastLoginIP { get; set; }
        [AutoSetCache]
        public decimal Points { get; set; }
        [AutoSetCache]
        public DateTime? LastWeixinSignInTime { get; set; }
        [AutoSetCache]
        public decimal Wallet { get; set; }
        [AutoSetCache]
        public System.DateTime AddTime { get; set; }
    }

    /// <summary>
    /// 用户工作流信息
    /// </summary>
    [Serializable]
    public class FullAccountWorkFlow
    {
        private const string KEY_FORMAT = "{0}@@@{1}"; //格式：[QuestionActivityId]@@@[AccountId]

        /// <summary>
        /// 获取Key中的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="questionActivityId"></param>
        /// <param name="accountId"></param>
        public static void GetKeyValues(string key, out int questionActivityId, out int accountId)
        {
            try
            {
                var values = key.Split(new[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                questionActivityId = int.Parse(values[0]);
                accountId = int.Parse(values[1]);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string Key
        {
            get { return GetKey(ActivityId, AccountId); }
        }
        /// <summary>
        /// 获取Key
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetKey(int activityId, int accountId)
        {
            return KEY_FORMAT.With(activityId, accountId);
        }

        public int AccountId { get; set; }
        public int ActivityId { get; set; }

        public Input Input { get; set; }
        public List<BaseModule> BaseModuleList { get; set; }
    }


    #endregion
}

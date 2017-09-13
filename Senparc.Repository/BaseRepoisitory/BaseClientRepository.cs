/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseClientRepository.cs

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
using Senparc.Core.Models;
using Senparc.Core.Utility;
using Senparc.Core.Extensions;
using Senparc.Core.Enums;
using StructureMap;

namespace Senparc.Repository
{
    public interface IBaseClientRepository<T> : IBaseRepository<T> where T : class,new() // global::System.Data.Objects.DataClasses.EntityObject, new()
    {
        ISqlClientFinanceData DB { get; }
    }

    public class BaseClientRepository<T> : BaseRepository<T>, IBaseClientRepository<T> where T : class,new() // global::System.Data.Objects.DataClasses.EntityObject, new()
    {
        public ISqlClientFinanceData DB
        {
            get
            {
                return base.BaseDB as ISqlClientFinanceData;
            }
        }

        public BaseClientRepository() : this(null) { }
        public BaseClientRepository(ISqlClientFinanceData db)
        {
            //System.Web.HttpContext.Current.Response.Write("-"+this.GetType().Name + "<br />");
            base.BaseDB = db ?? ObjectFactory.GetInstance<ISqlClientFinanceData>();
            _entitySetName = EntitySetKeys.Keys[typeof(T)];
        }

      
    }
}

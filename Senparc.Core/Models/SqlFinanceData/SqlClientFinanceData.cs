/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：SqlClientFinanceData.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using StructureMap;
using StructureMap.Pipeline;
using Senparc.Core.Extensions;
using System.Web;
using Senparc.Log;

namespace Senparc.Core.Models
{
    [PluginFamily("ClientDatabase", Scope = InstanceScope.Hybrid)]
    public interface ISqlClientFinanceData : ISqlBaseFinanceData
    {
        SenparcEntities DataContext { get; }
    }

    [Pluggable("ClientDatabase")]
    public class SqlClientFinanceData : SqlBaseFinanceData, ISqlBaseFinanceData, ISqlClientFinanceData
    {
        private SenparcEntities dataContext;
        private string lastTempDomainName = null;

        public SenparcEntities DataContext
        {
            get
            {
                return BaseDataContext as SenparcEntities;
            }
        }

        /// <summary>
        /// 关闭连接（长时间保持一个连接操作会导致数据库操作时间逐渐变长）
        /// </summary>
        public override void CloseConnection()
        {
            BaseDataContext.Database.Connection.Close();
            dataContext = null;
        }

        public override DbContext BaseDataContext
        {
            get
            {
                if (dataContext == null)
                {
                    //根据数据库类型不同，区分输出连接字符串。
                    string provider = "System.Data.SqlClient";
                    var connectionString = string.Format(@"metadata=res://*/Models.Senparc.csdl|res://*/Models.Senparc.ssdl|res://*/Models.Senparc.msl;provider={0};provider connection string='{1}'", provider, ConfigurationManager.AppSettings["ConnectionString"]);

                    dataContext = new SenparcEntities(connectionString);//TODO:当前采用注入可以保证HttpContext单例，如果要全局单例，可采用单件模式（需要先解决释放的问题）
                }
                else
                {
                    //LogUtility.WebLogger.Debug("DataContext已存在:" + dataContext.Connection.ConnectionString + ",HashCode:" + this.GetHashCode());
                }

                //var hashCode = dataContext.GetHashCode();
                //System.Web.HttpContext.Current.Response.Write(dataContext.GetHashCode() + "<br />");//测试同一Request只有一个SenparcEntities实例
                return dataContext;
            }

        }
    }
}

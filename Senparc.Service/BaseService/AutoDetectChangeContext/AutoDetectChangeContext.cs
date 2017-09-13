/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AutoDetectChangeContext.cs

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
using System.Threading.Tasks;
using Senparc.Core.Models;

namespace Senparc.Service
{
    /// <summary>
    /// 临时开放BaseDataContext.Configuration.AutoDetectChangesEnabled属性，用于大批量更新数据的环境，结束后还原到false状态。
    /// </summary>
    public class AutoDetectChangeContextWrap : IDisposable
    {
        public IBaseServiceData ServiceData { get; set; }
        //private ISqlBaseFinanceData _sqlFinanceData;


        public AutoDetectChangeContextWrap(IBaseServiceData serviceData)
        {
            //_service = service;
            //_service.BaseRepository.BaseDB.BaseDataContext.Configuration.AutoDetectChangesEnabled = true;
            //_sqlFinanceData = sqlFinanceData;
            ServiceData = serviceData;
            ServiceData.BaseData.BaseDB.BaseDataContext.Configuration.AutoDetectChangesEnabled = false;
            ServiceData.BaseData.BaseDB.ManualDetectChangeObject = true;
        }

        public void Dispose()
        {
            //_service.BaseRepository.BaseDB.BaseDataContext.Configuration.AutoDetectChangesEnabled = false;
            ServiceData.BaseData.BaseDB.BaseDataContext.Configuration.AutoDetectChangesEnabled = true;
            ServiceData.BaseData.BaseDB.ManualDetectChangeObject = false;
        }
    }

    public class CloseAutoDetectChangeContext : IDisposable
    {
        AutoDetectChangeContextWrap _wrap;
        public CloseAutoDetectChangeContext(AutoDetectChangeContextWrap wrap)
        {
            _wrap = wrap;
            _wrap.ServiceData.BaseData.BaseDB.BaseDataContext.Configuration.AutoDetectChangesEnabled = true;
        }

        public void Dispose()
        {
            _wrap.ServiceData.BaseData.BaseDB.BaseDataContext.Configuration.AutoDetectChangesEnabled = false;

        }
    }
}

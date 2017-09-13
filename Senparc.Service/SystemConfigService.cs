/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：SystemConfigService.cs

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
using Senparc.Core.Cache;
using Senparc.Core.Models;
using Senparc.Repository;
using Senparc.Log;
using StructureMap;

namespace Senparc.Service
{
    public interface ISystemConfigService : IBaseClientService<SystemConfig>
    {
        void RecycleAppPool();
    }

    public class SystemConfigService : BaseClientService<SystemConfig>, ISystemConfigService
    {
        public SystemConfigService(ISystemConfigRepository systemConfigRepo)
            : base(systemConfigRepo)
        {

        }


        //public void BackupDatabase(string domainName)
        //{
        //    string timeStamp = DateTime.Now.ToString("yyyyMMdd-HH-mm");//分钟
        //    //删除一分钟内备份的文件
        //    var dbDir = "~/App_Data/ClientDataBase/{0}/".With(domainName);
        //    var bakDir = "~/App_Data/ClientDataBase/{0}/Bak/".With(domainName);
        //    var files = Directory.GetFiles(HttpContext.Current.Server.MapPath(bakDir), "#SenparcCRM.config." + timeStamp + "*");
        //    foreach (var file in files)
        //    {
        //        System.IO.File.Delete(file);
        //    }

        //    string oldFilename = HttpContext.Current.Server.MapPath(dbDir+"#SenparcCRM.config");
        //    string newFilename = HttpContext.Current.Server.MapPath(bakDir+"#SenparcCRM.config") + "." + timeStamp + ".bak";
        //    System.IO.File.Copy(oldFilename, newFilename);
        //}

        //public void RestoreDatabase(string domainName,string fileName)
        //{
        //    var dbDir = "~/App_Data/ClientDataBase/{0}/".With(domainName);
        //    var bakDir = "~/App_Data/ClientDataBase/{0}/Bak/".With(domainName);

        //    string fullFilename = HttpContext.Current.Server.MapPath(bakDir + fileName);
        //    string runningDatabasePath = HttpContext.Current.Server.MapPath(dbDir + "#SenparcCRM.config");
        //    if (fileName.IsNullOrEmpty() || !File.Exists(fullFilename))
        //    {
        //        throw new Exception("备份数据库不存在！");
        //    }

        //    this.BackupDatabase(domainName);//备份当前数据库
        //    System.IO.File.Copy(fullFilename, runningDatabasePath, true);//还原
        //}

        //public void DeleteBackupDatabase(string domainName, string fileName, bool deleteAllBefore)
        //{
        //    var bakDir = "~/App_Data/ClientDataBase/{0}/Bak/".With(domainName);

        //    string fullFilename = HttpContext.Current.Server.MapPath(bakDir + fileName);

        //    if (!deleteAllBefore)
        //    {
        //        File.Delete(fullFilename);
        //    }
        //    else
        //    {
        //        var files = Directory.GetFiles(HttpContext.Current.Server.MapPath(bakDir)).OrderByDescending(z => z);
        //        bool startDelete = false;
        //        foreach (var file in files)
        //        {
        //            FileInfo fileInfo = new FileInfo(file);
        //            if (fileInfo.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                startDelete = true;
        //            }
        //            if (startDelete)
        //            {
        //                File.Delete(file);
        //            }
        //        }
        //    }
        //}

        public void RecycleAppPool()
        {
            string webConfigPath = HttpContext.Current.Server.MapPath("~/Web.config");
            System.IO.File.SetLastWriteTimeUtc(webConfigPath, DateTime.UtcNow);
        }

        public override void SaveObject(SystemConfig obj)
        {
            LogUtility.SystemLogger.InfoFormat("系统信息被编辑");

            base.SaveObject(obj);

            //删除缓存
            var systemConfigCache = StructureMap.ObjectFactory.GetInstance<IFullSystemConfigCache>();
            systemConfigCache.RemoveCache();
        }

        public override void DeleteObject(SystemConfig obj)
        {
            throw new Exception("系统信息不能被删除！");
        }
    }
}


/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：APP_RedPackage_ActivityService.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Repository;

namespace Senparc.Service
{
    public interface IAPP_RedPackage_ActivityService : IBaseClientService<APP_RedPackage_Activity>
    {
    }

    public class APP_RedPackage_ActivityService : BaseClientService<APP_RedPackage_Activity>,
        IAPP_RedPackage_ActivityService
    {
        public APP_RedPackage_ActivityService(IAPP_RedPackage_ActivityRepository appRedPackageActivityRepository)
            : base(appRedPackageActivityRepository)
        {
        }

        public override void SaveObject(APP_RedPackage_Activity obj)
        {
            var isInsert = base.IsInsert(obj);
            base.SaveObject(obj);
            LogUtility.WebLogger.InfoFormat("APP_RedPackage_Activity{2}：{0}（ID：{1}）", obj.Id, obj.Id,
                isInsert ? "新增" : "编辑");
        }

        public override void DeleteObject(APP_RedPackage_Activity obj)
        {
            LogUtility.WebLogger.InfoFormat("APP_RedPackage_Activity被删除：{0}（ID：{1}）", obj.Id, obj.Id);
            base.DeleteObject(obj);
        }
    }
}
using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Repository;

namespace Senparc.Service
{
    public interface IAPP_RedPackage_Activity_AwardService : IBaseClientService<APP_RedPackage_Activity_Award>
    {
    }

    public class APP_RedPackage_Activity_AwardService : BaseClientService<APP_RedPackage_Activity_Award>,
        IAPP_RedPackage_Activity_AwardService
    {
        public APP_RedPackage_Activity_AwardService(
            IAPP_RedPackage_Activity_AwardRepository appRedPackageActivityAwardRepository)
            : base(appRedPackageActivityAwardRepository)
        {
        }

        public override void SaveObject(APP_RedPackage_Activity_Award obj)
        {
            var isInsert = base.IsInsert(obj);
            base.SaveObject(obj);
            LogUtility.WebLogger.InfoFormat("APP_RedPackage_Activity_Award{2}：{0}（ID：{1}）", obj.Id, obj.Id, isInsert ? "新增" : "编辑");
        }

        public override void DeleteObject(APP_RedPackage_Activity_Award obj)
        {
            LogUtility.WebLogger.InfoFormat("APP_RedPackage_Activity_Award被删除：{0}（ID：{1}）", obj.Id, obj.Id);
            base.DeleteObject(obj);
        }
    }
}
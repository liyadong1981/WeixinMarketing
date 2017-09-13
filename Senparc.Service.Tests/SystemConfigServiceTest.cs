using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.Core.Utility;
using Senparc.Core.Config;
using System.IO;
using Senparc.Service;
using Senparc.Core.Models;
using Senparc.Core;

namespace Senparc.Service.Tests
{
    [TestClass]
    public class SystemConfigServiceTest : BaseServiceTest<SystemConfig>
    {
        private ISystemConfigService _systemConfigService;

        public SystemConfigServiceTest()
        {
            _systemConfigService = base.target as ISystemConfigService;
        }

        [TestMethod]
        public void InitTest()
        {
            var systemConfig = new SystemConfig()
            {
                Id = 19,
            };

            DateTime dt1 = DateTime.Now;
            var i = 0;
            for (; i < 50; i++)
            {
                var oldCount = _systemConfigService.GetCount(z => true);
                _systemConfigService.SaveObject(systemConfig);
                var newCount = _systemConfigService.GetCount(z => true);
                Assert.AreEqual(newCount, oldCount + 1);

                try
                {
                    _systemConfigService.DeleteObject(systemConfig);
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    Assert.AreEqual("系统信息不能被删除！", ex.Message);
                }
                finally
                {
                    _systemConfigService.BaseClientRepository.DB.DataContext.SystemConfigs.Remove(systemConfig);
                    _systemConfigService.BaseClientRepository.DB.DataContext.SaveChanges();
                }

                newCount = _systemConfigService.GetCount(z => true);
                Assert.AreEqual(oldCount, newCount);
            }
            DateTime dt2 = DateTime.Now;

            Console.WriteLine("执行{1}次测试时间：{0} MS", (dt2 - dt1).TotalMilliseconds, i);
        }
    }
}

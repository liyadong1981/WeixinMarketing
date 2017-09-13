using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.Core.Tests;
using Senparc.Core.Utility;
using Senparc.Core.Config;
using System.IO;
using Senparc.Service;
using Senparc.Core.Models;

namespace Senparc.Service.Tests
{
    [TestClass]
    public class BaseServiceTest<T> : BaseCoreTest where T : class,new()
    {
        protected IBaseClientService<T> target;

        public BaseServiceTest()
        {
            var interfaceName = "Senparc.Service.I" + typeof(T).Name + "Service,Senparc.Service";
            Type interfaceType = Type.GetType(interfaceName);
            target = StructureMap.ObjectFactory.GetInstance(interfaceType) as IBaseClientService<T>;
        }
    }
}

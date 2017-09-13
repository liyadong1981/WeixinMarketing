/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：Bootstrapper.cs

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
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap;
using Senparc.Core.Models;

namespace Senparc.Mvc.IoC
{

    public static class Bootstrapper
    {
        public static void ConfigureStructureMap()
        {
            //ObjectFactory.Initialize(x => x.AddRegistry(new DBServiceRegistry()));
            ObjectFactory.Initialize(InitializeStructureMap);
            //ObjectFactory.Initialize(x => x.AddRegistry(new DomainRegistry()));
        }

        private static void InitializeStructureMap(IInitializationExpression x)
        {
            x.Scan(y =>
            {
                y.Assembly("Senparc.Core");
                y.Assembly("Senparc.Mvc");
                y.Assembly("Senparc.Repository");
                y.Assembly("Senparc.Service");
                //y.Assembly("Senparc.Models");
                y.WithDefaultConventions();
            });

        }
    }
}

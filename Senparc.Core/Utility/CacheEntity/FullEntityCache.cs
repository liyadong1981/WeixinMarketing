/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：FullEntityCache.cs

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
using System.Reflection;
using Senparc.Log;

namespace Senparc.Core.Utility
{
    public static class FullEntityCache
    {
        public static void SetFullEntityCache<TFullEntity, TEntity>(TFullEntity fullEntity, TEntity entity)
        {
            if (fullEntity == null || entity == null)
            {
                throw new Exception("参数不可以为NULL");
            }

            try
            {

                var fullEntityType = fullEntity.GetType();
                var entityType = entity.GetType();
                var props =
                    fullEntityType.GetProperties();
                foreach (var p in props)
                {
                    //获得当前属性的特性
                    AutoSetCacheAttribute m =
                        Attribute.GetCustomAttribute(p, typeof(AutoSetCacheAttribute)) as AutoSetCacheAttribute;
                    if (m != null)
                    {
                        //允许自动复制
                        //获取原始实体值
                        var entityProp = entityType.GetProperty(p.Name);
                        if (entityProp == null || entityProp.PropertyType != p.PropertyType)
                        {
                            throw new Exception("原始实体没有相同类型和名称的属性存在：" + p.Name);
                        }
                        p.SetValue(fullEntity, entityProp.GetValue(entity, null), null);
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtility.SystemLogger.Debug("跟踪Cache错误：" + ex.Message, ex);
                throw ex;
            }
        }
    }
}

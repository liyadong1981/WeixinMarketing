/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：EntitySetKeys.cs

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

namespace Senparc.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class EntitySetKeys
    {
        public static EntitySetKeysDictionary Keys = new EntitySetKeysDictionary();

        //public static string GetEntitySetName(this Type entityType)
        //{
        //    return Keys[entityType];
        //}
    }
    /// <summary>
    /// 与ORM实体类对应的实体集
    /// </summary>
    public class EntitySetKeysDictionary : Dictionary<Type, string>
    {
        public EntitySetKeysDictionary()
        {
            //初始化的时候从ORM中自动读取实体集名称及实体类别名称
            var clientProperties = typeof(Models.SenparcEntities).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            var properities = new List<PropertyInfo>();
            properities.AddRange(clientProperties);

            foreach (var prop in properities)
            {
                try
                {
                    //ObjectQuery，ObjectSet for EF4，DbSet for EF Code First
                    if (prop.PropertyType.Name.IndexOf("DbSet") != -1 && prop.PropertyType.GetGenericArguments().Length > 0)
                    {
                        this[prop.PropertyType.GetGenericArguments()[0]] = prop.Name;//获取第一个泛型
                    }
                }
                catch { }
            }

        }

        new public string this[Type entityType]
        {
            get
            {
                if (!base.ContainsKey(entityType))
                {
                    throw new Exception("未找到实体类型");
                }
                return base[entityType];
            }
            set
            {
                base[entityType] = value;
            }
        }
    }
}

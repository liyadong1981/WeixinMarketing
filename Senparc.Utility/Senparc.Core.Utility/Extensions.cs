/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：Extensions.cs

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
using System.ComponentModel;
using System.Reflection;

namespace Senparc.Core.Utility
{
    public static class Extensions
    {
        /// <summary>
        /// 获取翻页时跳过的记录数
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageCount">每页记录数</param>
        /// <returns></returns>
        public static int GetSkipRecord(int pageIndex, int pageCount)
        {
            return (pageIndex - 1) * pageCount;
        }

      

        /// <summary>
        /// 获取枚举成员，转为Dictionary类型
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="useDescription">是否使用枚举类型的描述</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDictionaryForEnums(this Type enumType, bool useDescription = false, bool addBlankOption = false, string blankOptionText = null)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("此对象不是Enum类型！");
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (addBlankOption)
            {
                dic.Add("", blankOptionText ?? "");//添加空白项
            }
            foreach (int item in Enum.GetValues(enumType))
            {
                string name = Enum.GetName(enumType, item);
                if (useDescription)
                {
                    FieldInfo fi = enumType.GetField(Enum.GetName(enumType, item));
                    var dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                    if (dna != null)
                    {
                        name = dna.Description;
                    }
                }

                name = name ?? Enum.GetName(enumType, item);
                dic.Add(item.ToString(), name);
            }
            return dic;
        }

        public static string GetDescriptionForEnum(this Type enumType, int item)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("此对象不是Enum类型！");
            }
            string name = null;
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, item));
            var dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            if (dna != null)
            {
                name = dna.Description;
            }
            return name;
        }

        /// <summary>
        /// 判断baseType是否为type的基类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsInherit(this System.Type type, System.Type baseType)
        {
            if (type.BaseType == null) return false;
            if (type.BaseType == baseType) return true;
            return IsInherit(type.BaseType, baseType);
        }
    }
}

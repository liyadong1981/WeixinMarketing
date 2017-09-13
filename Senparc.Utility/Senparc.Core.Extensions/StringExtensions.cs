/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：StringExtensions.cs

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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using Senparc.Core.Utility;
using Newtonsoft;

namespace Senparc.Core.Extensions
{
    public static class StringExtensions
    {
        public static string With(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(str);
        }

        public static T ShowWhenNullOrEmpty<T>(this T obj, T defaultConent)
        {
            if (obj == null)
            {
                return defaultConent;
            }
            else if (obj is String && obj.ToString() == "")
            {
                return defaultConent;
            }
            else
            {
                return obj;
            }
        }

        public static string UrlEncode(this string str)
        {
            return System.Web.HttpUtility.UrlEncode(str);
        }

        public static string UrlDecode(this string str)
        {
            return System.Web.HttpUtility.UrlDecode(str);
        }



        /// <summary>
        /// 把数据转换为Json格式（使用Newtonsoft.Json.dll）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

    }
}

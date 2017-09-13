/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：Enums.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Senparc.Core.Enums
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheType
    {
        Location,
        Memcached,
        Redis
    }

    #region 实体类型

    #endregion

    public enum MessageType
    {
        success,
        error,
        information,
        attention
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum Sex
    {
        未设置 = 0,
        男 = 1,
        女 = 2,
    }

    /// <summary>
    /// 排序类型
    /// </summary>
    public enum OrderingType
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Meta类型
    /// </summary>
    public enum MetaType
    {
        keywords,
        description
    }

    public enum Module_Type
    {
        ContrastPic,
        RedPackageResult,
        GradeResult,
        Register,
        RegisterImg,
        EmotionApi,
        PicContainerApi,
        VisionApi,
        FaceApi,
    }

    #region 实体属性

    /// <summary>
    /// 活动类型
    /// </summary>
    public enum Activity_Type
    {
        摇一摇红包 = 0,
        刮刮乐 = 1,
        大转盘 = 2,
    }

    /// <summary>
    /// 活动状态
    /// </summary>
    public enum Activity_State
    {
        开始 = 0,
        结束 = 1,
    }

    /// <summary>
    /// 订单状态
    /// </summary>
    public enum Order_Status
    {
        未支付 = 0,
        已支付 = 1,
        已取消 = 2,
        已冻结 = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Order_PayType
    {
        微信支付 = 0,
        其他 = 1
    }

    public enum Order_Type
    {
        摇一摇红包=0
    }
    /// <summary>
    /// 用户类型
    /// </summary>
    public enum Account_Type
    {
        普通用户 = 0,
    }

    #endregion
}
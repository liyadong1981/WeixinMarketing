/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：PicContainerApiModule.cs

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
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision.Contract;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;

namespace Senparc.Core.Models.WorkFlowModules
{
    [Serializable]
    public class VisionParameters
    {
        public string Pic { get; set; }
        public List<VisionTags> Result { get; set; }
    }
    [Serializable]
    public class VisionTags
    {
        public VisionTags(double confidence, string hint, string name)
        {
            Confidence = confidence;
            Hint = hint;
            Name = name;
        }
        public double Confidence { get; set; }
        public string Hint { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// 图片容器API
    /// </summary>
    [Serializable]
    public class PicContainerApiModule : BaseApiModule

    {
        public VisionParameters Parameters { get; set; }

        public PicContainerApiModule()
            : base("图片API", Module_Type.PicContainerApi)
        {
            //base.Input.Add(new BaseParameterSetting(ParameterSettingType.SetPic, "图片", ""));
            base.Output.Add(new PicContainerParameterSetting("图片评分", null));
        }
    }
}
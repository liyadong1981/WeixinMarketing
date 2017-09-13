/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：VisionApi.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Senparc.Core.Config;

namespace Senparc.AzureCognitiveService
{
    /// <summary>
    /// 认知服务计算机视觉API
    /// </summary>
    public static class VisionApi
    {
        /// <summary>
        /// KEY 申请CognitiveServices 
        /// </summary>
        //private static string subscriptionKey = "a186258f17254f8ca38ca85f4ef1926b";

        private static readonly string apiRoot = "https://api.projectoxford.ai/vision/v1.0/"; //Azure上对应的接口
        #region 单例

        private static VisionServiceClient Instance
        {
            get
            {
                return Nested.instance; //返回Nested类中的静态成员instance
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            //将instance设为一个初始化的VisionServiceClient新实例
            internal static readonly VisionServiceClient instance = new VisionServiceClient(SiteConfig.VisionKey);
        }

        #endregion

        /// <summary>
        /// 调用计算机视觉API
        /// </summary>
        /// <param name="imageFilePath">本地图片地址</param>
        /// <param name="domainModel"></param>
        /// <returns></returns>
        public static AnalysisInDomainResult UploadAndAnalyzeInDomainImage(string imageFilePath, Model domainModel)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var analysisResult =
                        Task.Run(() => Instance.AnalyzeImageInDomainAsync(imageFileStream, domainModel)).Result;
                    return analysisResult;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 调用计算机视觉API
        /// </summary>
        /// <param name="imageUrl">Url</param>
        /// <param name="domainModel"></param>
        /// <returns></returns>
        public static AnalysisInDomainResult GetAnalyzeInDomainUrl(string imageUrl, Model domainModel)
        {
            try
            {
                var analysisResult = Task.Run(() => Instance.AnalyzeImageInDomainAsync(imageUrl, domainModel)).Result;
                return analysisResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Uploads the image to Project Oxford and generates tags
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <returns></returns>
        public static AnalysisResult UploadAndGetTagsForImage(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var analysisResult = Task.Run(() => Instance.GetTagsAsync(imageFileStream)).Result;
                    return analysisResult;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Sends a url to Project Oxford and generates tags for it
        /// </summary>
        /// <param name="imageUrl">The url of the image to generate tags for</param>
        /// <returns></returns>
        public static AnalysisResult GenerateTagsForUrl(string imageUrl)
        {
            var analysisResult = Task.Run(() => Instance.GetTagsAsync(imageUrl)).Result;
            return analysisResult;
        }

    }
}
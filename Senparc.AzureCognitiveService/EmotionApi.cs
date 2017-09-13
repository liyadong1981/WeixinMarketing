/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：EmotionApi.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Senparc.Core.Config;

namespace Senparc.AzureCognitiveService
{
    /// <summary>
    /// 认知服务情绪识别API
    /// </summary>
    public static class EmotionApi
    {
        internal class EmotionResultDisplay
        {
            public string EmotionString { get; set; }
            public float Score { get; set; }
            public int OriginalIndex { get; set; }
        }

        /// <summary>
        /// KEY[Slave] 申请CognitiveServices 
        /// </summary>
        //private static string subscriptionMaterKey = "d72ba3f834984eea9fe0d6908756b5f7";
        /// <summary>
        /// KEY[Mater] 申请CognitiveServices 
        /// </summary>
        //private static string subscriptionSlaveKey = "9368e4cd242c4761a7b5fe8e7087c0ca";

        //API Key集合
        private static readonly string[] subscriptionKeyArray = { "d72ba3f834984eea9fe0d6908756b5f7", "9368e4cd242c4761a7b5fe8e7087c0ca" };

        private static readonly TimeSpan QueryWaitTime = TimeSpan.FromSeconds(20);
        /// <summary>
        /// 随机API Key
        /// </summary>
        private static Random random = new Random();

        private static readonly string apiRoot = "https://api.cognitive.ai/emotion/v1.0"; //Azure上对应的接口

        #region 单例

        public static EmotionServiceClient Instance
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

            //将instance设为一个初始化的EmotionServiceClient新实例
            internal static readonly EmotionServiceClient instance = new EmotionServiceClient(SiteConfig.EmotionKey);
        }

        #endregion

        /// <summary>
        /// Uploads the video to Project Oxford and detects emotions
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <returns></returns>
        public static async Task<Emotion[]> UploadAndStreamDetectEmotionsAsync(string imageFilePath)
        {
            // Create Project Oxford Emotion API Service client


            try
            {
                var index = random.Next(0, subscriptionKeyArray.Length);
                var renderKey = subscriptionKeyArray[index];
                EmotionServiceClient emotionServiceMaterClient = new EmotionServiceClient(renderKey);
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    //Emotion[] emotionResult = null;
                    // Detect the emotions in the URL
                    var emotionResult = await emotionServiceMaterClient.RecognizeAsync(imageFileStream);
                    if (emotionResult == null)
                    {
                        renderKey = subscriptionKeyArray.FirstOrDefault(z => z != renderKey);
                        var emotionServiceSlaveClient = new EmotionServiceClient(renderKey);
                        emotionResult = await emotionServiceSlaveClient.RecognizeAsync(imageFileStream);
                    }
                    //var emotionResult = Task.Run(() =>
                    //     Instance.RecognizeAsync(imageFileStream)).Result;
                    return emotionResult;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Uploads the image to Project Oxford and detect emotions.
        /// </summary>
        /// <param name="url">The image file path.</param>
        /// <returns></returns>
        public static async Task<Emotion[]> GetDetectEmotionsUrl(string url)
        {
            // Create Project Oxford Emotion API Service client
            //EmotionServiceClient emotionServiceClient = new EmotionServiceClient(SubscriptionKey);

            try
            {
                // Detect the emotions in the URL
                //Emotion[] emotionResult = await Instance.RecognizeAsync(url);
                var emotionResult = await Instance.RecognizeAsync(url);
                return emotionResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Uploads the video to Project Oxford and detects emotions.
        /// </summary>
        /// <param name="videoFilePath">The video file path.</param>
        /// <returns></returns>
        public static async Task<VideoAggregateRecognitionResult> UploadAndVideoDetectEmotions(string videoFilePath)
        {
            // Create Project Oxford Emotion API Service client
            //EmotionServiceClient emotionServiceClient = new EmotionServiceClient(SubscriptionKey);

            try
            {
                using (Stream videoFileStream = File.OpenRead(videoFilePath))
                {
                    // Upload the video, and tell the server to start recognizing emotions
                    //VideoEmotionRecognitionOperation videoOperation =
                    var videoOperation = await Instance.RecognizeInVideoAsync(videoFileStream);

                    // Starts querying service status
                    VideoOperationResult result;
                    while (true)
                    {
                        result = await Instance.GetOperationResultAsync(videoOperation);
                        if (result.Status == VideoOperationStatus.Succeeded ||
                            result.Status == VideoOperationStatus.Failed)
                        {
                            break;
                        }
                        await Task.Delay(QueryWaitTime);
                    }
                    // Processing finished, checks result
                    if (result.Status == VideoOperationStatus.Succeeded)
                    {
                        // Get the processing result by casting to the actual operation result
                        var aggregateResult =
                            ((VideoOperationInfoResult<VideoAggregateRecognitionResult>)result).ProcessingResult;
                        return aggregateResult;
                    }
                    else
                    {
                        // Failed
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
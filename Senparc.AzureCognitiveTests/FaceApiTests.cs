using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.AzureCognitiveService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.AzureCognitiveTests;
using ServiceStack;

namespace Senparc.AzureCognitiveService.Tests
{
    [TestClass()]
    public class FaceApiTests : BaseCognitiveTest
    {
        [TestMethod()]
        public void UploadAndStreamDetectFace()
        {
            var imagePath = @"C:\Users\Administrator\Desktop\my.jpg"; //@"E:\Senparc\AzureDemo\WebSite\Senparc.Web\Upload\Emotion\happy.jpg";
            var faces = FaceApi.UploadAndStreamDetectFace(imagePath);
            Assert.IsNotNull(faces);
            foreach (var item in faces)
            {
                Console.WriteLine($"========Face：{item.FaceAttributes.ToJson()}");
            }
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ProjectOxford.Vision.Contract;
using Senparc.AzureCognitiveTests;

namespace Senparc.AzureCognitiveService.Tests
{
    [TestClass()]
    public class VisionApiTests : BaseCognitiveTest
    {
        string imageFilePath = @"E:\Senparc\AzureDemo\WebSite\Senparc.Web\Upload\Emotion\happy.jpg";
        [TestMethod()]
        public void UploadAndAnalyzeInDomainImageTest()
        {
            //var imageFilePath = @"E:\Senparc\AzureDemo\WebSite\Senparc.Web\Upload\Emotion\happy.jpg";
            var model = new Model() { Name = "categories", Categories = new[] { "人" } };
            var result = VisionApi.UploadAndAnalyzeInDomainImage(imageFilePath, model);
            Assert.IsNotNull(result);
            //Console.WriteLine($"==============={result.ToJson()}");
        }

        [TestMethod()]
        public void UploadAndGetTagsForImageTest()
        {
            var result = VisionApi.UploadAndGetTagsForImage(imageFilePath);
            Assert.IsNotNull(result);
           // Console.WriteLine($"==============={result.ToJson()}");
        }
    }
}
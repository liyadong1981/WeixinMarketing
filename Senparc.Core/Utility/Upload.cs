/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：Upload.cs

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
using System.IO;
using System.Web;
using Senparc.Core.Extensions;
using Senparc.Core.Models;

namespace Senparc.Core.Utility
{
    public class Upload
    {
        #region 私有（内部）方法

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="saveOnServerPath">保存到服务器路径（"~/upload/"下面）</param>
        /// <param name="file">HttpPostedFileBase</param>
        /// <param name="fileNameOnServer">保存文件名（不包含扩展名）</param>
        /// <param name="limit">限制大小（KB）</param>
        /// <param name="isDel">是否删除已存在</param>
        /// <returns></returns>
        private static string UploadFile_Img(string saveOnServerPath, HttpPostedFileBase file, string fileNameOnServer, long limit, bool isDel, string[] allowedExtension)
        {
            allowedExtension = allowedExtension ?? new string[] { ".gif", ".png", ".bmp", ".jpg" };//允许扩展名
            string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            //long limit = 22000000;//限制大小（KB）

            //bool fileOK = false;
            //则判断文件类型是否符合要求
            if (allowedExtension.Contains(fileExtension))
            {
                //fileOK = true;

                //限制大小  220M  ——by TNT2
                if (file.ContentLength < limit * 1024)
                {
                    if (!saveOnServerPath.EndsWith("/"))
                    {
                        saveOnServerPath += "/";
                    }
                    return UploadFile("~/upload/" + saveOnServerPath, fileNameOnServer, file, limit, isDel);
                }
                else
                {
                    return "只能上传" + limit + "KB以内的文件！此文件大小：" + file.ContentLength / 1024 + " KB";
                }
            }
            else
            {
                return "只能上传图片文件！";
            }
        }

        /// <summary>
        /// 私有方法，上传文件，防止外部调用
        /// </summary>
        /// <param name="savePathStr"></param>
        /// <param name="file">HttpPostedFileBase</param>
        /// <param name="fileNameOnServer">保存文件名（不包含扩展名）</param>
        /// <param name="limit">限制大小（KB）</param>
        /// <param name="isDel">是否删除已存在</param>
        /// <returns></returns>
        private static string UploadFile(string savePathStr, string fileNameOnServer, HttpPostedFileBase file, long limit, bool isDel, string[] allowedExtension = null)
        {
            //上传    ——by TNT2;
            string saveFileName = string.Empty;//服务器上地址,如果成功，返回正确地址


            string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (allowedExtension != null && !allowedExtension.Contains(fileExtension))
            {
                return "为确保系统安全，此文件类型（{0}）被禁止上传，如确实需要上传，请联系客服。".With(fileExtension);
            }

            //调用SaveAs方法，实现上传，并显示相关信息    ——by TNT2

            //总限制大小  220M  ——by TNT2
            if (file.ContentLength < limit * 1024)
            {
                //实现上传    ——by TNT2
                //获取给予运用程序根文件夹的绝对路径    ——by TNT2
                saveFileName = fileNameOnServer + Path.GetExtension(file.FileName).ToLower();//保存文件名
                string savePhyicalPath = System.Web.HttpContext.Current.Server.MapPath(savePathStr);
                string savePhyicalFilePath = Path.Combine(savePhyicalPath, saveFileName);//服务器上的保存物理路径
                string saveApplicationPath = Path.Combine(GetFullApplicationPathFromVirtualPath(savePathStr), saveFileName);//文件的网站绝对地址

                //保存文件
                if (isDel && File.Exists(savePhyicalFilePath))
                {
                    File.Delete(savePhyicalFilePath);//先删除
                }

                if (!Directory.Exists(savePhyicalPath))
                {
                    Directory.CreateDirectory(savePhyicalPath);
                }
                file.SaveAs(savePhyicalFilePath);

                //返回图片的网站绝对地址
                return saveApplicationPath;
            }
            else
            {
                return "只能上传" + limit + "KB以内的文件！此文件大小：" + file.ContentLength / 1024 + " KB";
            }

            //return saveFileName;
        }

        /// <summary>
        /// 从应用程序虚拟路径，获取应用程序路径（相对网站根目录，/开头）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullApplicationPathFromVirtualPath(string virtualPath)
        {
            return virtualPath.Replace("~/", HttpContext.Current.Request.ApplicationPath);
        }

        #endregion

        /*  以上为私有方法  */

        public static string UpLoadPic(HttpPostedFileBase file)
        {
            string newFilename = DateTime.Now.Ticks.ToString();
            string[] allowedExtension = new string[] { ".jpg", ".bmp", ".png", ".jpeg", ".gif" };
            string uploadResult = UploadFile_Img("ApiPic", file, newFilename, 2 * 1024, true, allowedExtension);//执行上传
            return uploadResult;
        }

        public static string UpLoadProductPic(HttpPostedFileBase file)
        {
            string newFilename = DateTime.Now.Ticks.ToString();
            string[] allowedExtension = new string[] { ".jpg", ".bmp", ".png", ".jpeg", ".gif" };
            string uploadResult = UploadFile_Img("ProductPic", file, newFilename, 2 * 1024, true, allowedExtension);//执行上传
            return uploadResult;
        }

        /// <summary>
        /// 察看上传是否成功
        /// </summary>
        /// <param name="saveFileName">上传返回的字符串，如果“/”开头，则表示上传成功，不然里面包含的是错误信息</param>
        /// <returns></returns>
        public static bool CheckUploadSuccessful(string saveFileName)
        {
            if (saveFileName != null && saveFileName.StartsWith("/"))
                return true;
            else
                return false;
        }
    }
}

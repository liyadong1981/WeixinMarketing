/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：ContentBox.cs

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
using System.Web.Mvc;

namespace Senparc.Web.UI
{
    public class ContentBox : IDisposable
    {
        // Fields
        private bool _disposed;
        private readonly ViewContext _viewContext;
        private readonly TextWriter _writer;
        private bool _showDefaultTabContainer;

        public ContentBox(ViewContext viewContext, bool showDefaultTabContainer)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }
            this._viewContext = viewContext;
            this._writer = viewContext.Writer;
            this._showDefaultTabContainer = showDefaultTabContainer;
        }

        public void EndCotnentBox()
        {
            this.Dispose(true);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                string contentBoxFooter =
@"
    </div>
</div>";
                this._disposed = true;
                this._writer.Write(contentBoxFooter);
            }
        }
    }
}

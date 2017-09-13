/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AccountRepository.cs

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
using Senparc.Core.Models;

namespace Senparc.Repository
{
    /* 说明：这里做接口是为了可以扩展更多类型的数据库或策略、Provider等，如果确定只有一类数据Provider，
     * 可以直接使用类，不使用接口，书中的案例不需要接口，此处只是给大家一个演示
     */

    public interface IAccountRepository : IBaseClientRepository<Account>
    {
    }

    public class AccountRepository : BaseClientRepository<Account>, IAccountRepository
    {

    }
}


/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：IBaseService.cs

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
using Senparc.Core.Enums;
using Senparc.Repository;
using System.Linq.Expressions;

namespace Senparc.Service
{
    public interface IBaseService<T> : IBaseServiceData where T : class,new()// global::System.Data.Objects.DataClasses.EntityObject, new()
    {
        IBaseRepository<T> BaseRepository { get; set; }
        bool IsInsert(T obj);
        T GetObject(long id, string[] includes = null);
        T GetObject(Guid guid, string[] includes = null);
        T GetObject(Expression<Func<T, bool>> where, string[] includes = null);
        T GetObject<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null);
        PagedList<T> GetFullList<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null);
        PagedList<T> GetObjectList<TK>(int pageIndex, int pageCount, Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null);
        int GetCount(Expression<Func<T, bool>> where, string[] includes = null);
        decimal GetSum(Expression<Func<T, bool>> where, Func<T, decimal> sum, string[] includes = null);
        /// <summary>
        /// 强制将实体设置为Modified状态
        /// </summary>
        /// <param name="obj"></param>
        void TryDetectChange(T obj);
        void SaveObject(T obj);
        void DeleteObject(long id);
        void DeleteObject(Guid guid);
        void DeleteObject(T obj);
        void DeleteAll(IEnumerable<T> objects);
    }
}

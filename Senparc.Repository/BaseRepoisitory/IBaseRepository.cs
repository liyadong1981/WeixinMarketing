/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：IBaseRepository.cs

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
using Senparc.Core;
using Senparc.Core.Enums;
using System.Linq.Expressions;

namespace Senparc.Repository
{
    public interface IBaseRepository<T> : IBaseData where T : class,new()// global::System.Data.Objects.DataClasses.EntityObject, new()
    {
        ISqlBaseFinanceData BaseDB { get; set; }
        bool IsInsert(T obj);
        IQueryable<T> GeAll<TK>(Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null);
        T GetObjectById(long id, string[] includes = null);
        //T GetObjectByGuid(Guid guid, string[] includes = null);
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="where">搜索条件</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount">当pageCount小于等于0时不分页</param>
        /// <returns></returns>
        PagedList<T> GetObjectList<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, int pageIndex, int pageCount, string[] includes = null);
        T GetFirstOrDefaultObject(Expression<Func<T, bool>> where, string[] includes = null);
        T GetFirstOrDefaultObject<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null);
        int ObjectCount(Expression<Func<T, bool>> where, string[] includes = null);
        decimal GetSum(Expression<Func<T, bool>> where, Func<T, decimal> sum, string[] includes = null);
        //object ObjectSum(Expression<Func<T, bool>> where, Func<T, object> sumBy, string[] includes);
        void Add(T obj);
        void Update(T obj);
        /// <summary>
        /// 此方法会自动判断应当执行更新(Update)还是添加(Add)
        /// </summary>
        /// <param name="obj"></param>
        void Save(T obj);
        void Delete(T obj);
        //void DeleteAll(IEnumerable<T> objs);
        void SaveChanges();
    }
}

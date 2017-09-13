/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseService.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Senparc.Repository;
using Senparc.Core.Enums;
using Senparc.Core.Models;
using System.Linq.Expressions;

namespace Senparc.Service
{
    public class BaseService<T> : BaseServiceData, IBaseService<T> where T : class,new()// global::System.Data.Objects.DataClasses.EntityObject, new()
    {
        public IBaseRepository<T> BaseRepository { get; set; }

        public BaseService(IBaseRepository<T> repo)
            : base(repo)
        {
            BaseRepository = repo;
        }


        public virtual bool IsInsert(T obj)
        {
            return BaseRepository.IsInsert(obj);
        }

        public virtual T GetObject(long id, string[] includes = null)
        {
            return BaseRepository.GetObjectById(id, includes);
        }

        public virtual T GetObject(Guid guid, string[] includes = null)
        {
            throw new Exception("暂不支持");
            //return BaseRepository.GetObjectByGuid(guid, includes);
        }

        public T GetObject(Expression<Func<T, bool>> where, string[] includes = null)
        {
            return BaseRepository.GetFirstOrDefaultObject(where, includes);
        }

        public T GetObject<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null)
        {
            return BaseRepository.GetFirstOrDefaultObject(where, orderBy, orderingType, includes);
        }

        public PagedList<T> GetFullList<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null)
        {
            return this.GetObjectList(0, 0, where, orderBy, orderingType, includes);
        }

        public virtual PagedList<T> GetObjectList<TK>(int pageIndex, int pageCount, Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, string[] includes = null)
        {
            return BaseRepository.GetObjectList(where, orderBy, orderingType, pageIndex, pageCount, includes);
        }

        public virtual int GetCount(Expression<Func<T, bool>> where, string[] includes = null)
        {
            return BaseRepository.ObjectCount(where, includes);
        }

        public virtual decimal GetSum(Expression<Func<T, bool>> where, Func<T, decimal> sum, string[] includes = null)
        {
            return BaseRepository.GetSum(where, sum, includes);
        }

        /// <summary>
        /// 强制将实体设置为Modified状态
        /// </summary>
        /// <param name="obj"></param>
        public virtual void TryDetectChange(T obj)
        {
            if (!IsInsert(obj))
            {
                BaseRepository.BaseDB.BaseDataContext.Entry(obj).State = EntityState.Modified;
            }
        }

        public virtual void SaveObject(T obj)
        {
            if (BaseRepository.BaseDB.ManualDetectChangeObject)
            {
                TryDetectChange(obj);
            }
            BaseRepository.Save(obj);
        }

        public virtual void DeleteObject(long id)
        {
            T obj = this.GetObject(id, null);
            this.DeleteObject(obj);
        }

        public virtual void DeleteObject(Guid guid)
        {
            T obj = this.GetObject(guid, null);
            this.DeleteObject(obj);
        }

        public virtual void DeleteObject(T obj)
        {
            BaseRepository.Delete(obj);
        }

        public virtual void DeleteAll(IEnumerable<T> objects)
        {
            var list = objects.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                this.DeleteObject(list[i]);
            }
        }
    }
}

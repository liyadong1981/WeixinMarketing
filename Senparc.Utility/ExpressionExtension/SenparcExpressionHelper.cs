/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：SenparcExpressionHelper.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Senparc.Utility
{
    public class SenparcExpressionHelper<TEntity> where TEntity : class
    {
        public ValueCompare<TEntity> ValueCompare { get; set; }


        public SenparcExpressionHelper()
        {
            ValueCompare = new ValueCompare<TEntity>();
        }

        /// <summary>
        /// 生成where表达式
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> BuildWhereExpression()
        {
            if (ValueCompare.ExpressionBody==null)
            {
                //不需要查询
                Expression<Func<TEntity, bool>> returnTrue = z => true;
                return returnTrue;
            }
            var where = Expression.Lambda<Func<TEntity, bool>>(ValueCompare.ExpressionBody, new[] { ValueCompare.PE });
            return where;
        }
    }

    public class ValueCompare<TEntity>
    {
        /// <summary>
        /// 统一重新绑定参数Expression.Parameter
        /// </summary>
        private sealed class ParameterRebinder : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            public ParameterRebinder(ParameterExpression parameter)
            { this._parameter = parameter; }

            protected override Expression VisitParameter(ParameterExpression p)
            { return base.VisitParameter(this._parameter); }
        }

        public Expression ExpressionBody { get; set; }
        public ParameterExpression PE { get; set; }
        private ParameterRebinder _parameterRebinder;
        public ValueCompare()
        {
            PE = Expression.Parameter(typeof(TEntity), "z");
            _parameterRebinder = new ParameterRebinder(PE);
        }

        public ValueCompare<TEntity> Contains(ValueCompare<TEntity> vc)
        {
            return this;
        }

        //public ValueCompare<TEntity> Create(Expression<Func<TEntity, bool>> where)
        //{
        //    ExpressionBody = where;
        //    return this;
        //}


        public ValueCompare<TEntity> Create(Expression<Func<TEntity, bool>> filter)
        {
            var getterBody = _parameterRebinder.Visit(filter.Body);//统一参数
            ExpressionBody = getterBody;

            if (ExpressionBody==null)
            {
                throw new Exception("ExpressionBody为NULL");
            }
            return this;
        }

        
        public ValueCompare<TEntity> AndAlso(bool combineWhenTrue,Expression<Func<TEntity, bool>> filter)
        {
            return Combine(combineWhenTrue, ExpressionType.AndAlso, filter);
        }

        public ValueCompare<TEntity> OrElse(bool combineWhenTrue, Expression<Func<TEntity, bool>> filter)
        {
            return Combine(combineWhenTrue, ExpressionType.OrElse, filter);
        }

        public ValueCompare<TEntity> Combine(ExpressionType expressionType, Expression<Func<TEntity, bool>> filter)
        {
            if (ExpressionBody == null)
            {
                return Create(filter);
            }

            Expression left = ExpressionBody;
            Expression right = _parameterRebinder.Visit(filter.Body);//统一参数

            ExpressionBody = Expression.MakeBinary(expressionType, left, right);
            return this;
        }

        public ValueCompare<TEntity> Combine(bool combineWhenTrue, ExpressionType expressionType, Expression<Func<TEntity, bool>> filter)
        {
            if (!combineWhenTrue)
            {
                return this;
            }

            return Combine(expressionType, filter);
        }

    }
}

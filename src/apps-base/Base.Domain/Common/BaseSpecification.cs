using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Base.Domain.Common
{
    public abstract class BaseSpecification<T> : ISpecification<T>
            where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; protected set; }

        public List<Expression<Func<T, object>>> Includes { get; protected set; } = new List<Expression<Func<T, object>>>();

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }



        #region Criteria 扩展 (为了让Criteria能动态的添加Express条件,如果有不方便的情景,可以不使用)
        /// <summary>
        /// 为Criteria建立更加简便的Express拼接方式
        /// </summary>
        protected Expression<Func<T, bool>> CriteriaPredicate = PredicateBuilder.True<T>();

        protected virtual void AppendCriteriaAdd(Expression<Func<T, bool>> expression)
        {
            CriteriaPredicate = CriteriaPredicate.And(expression);
        }

        protected virtual void AppendCriteriaOr(Expression<Func<T, bool>> expression)
        {
            CriteriaPredicate = CriteriaPredicate.Or(expression);
        }
        #endregion
    }
}

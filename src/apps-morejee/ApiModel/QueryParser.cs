using ApiModel.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiModel
{
    public class QueryParser
    {
        public static List<Expression<Func<T, bool>>> Parse<T>(string q)
           where T : class, new()
        {
            var wheres = new List<Expression<Func<T, bool>>>();
            if (!string.IsNullOrWhiteSpace(q))
            {
                try
                {
                    var entityType = typeof(T);
                    var properties = entityType.GetProperties();
                    var filterArr = q.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in filterArr)
                    {
                        var fieldArr = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var field = fieldArr[0];
                        var operate = fieldArr[1];
                        var value = string.Join(' ', fieldArr.Skip(2).Take(fieldArr.Length - 2));

                        var propertyName = string.Empty;

                        #region 校验字段信息
                        for (int idx = properties.Length - 1; idx >= 0; idx--)
                        {
                            var propName = properties[idx].Name.ToString();
                            if (propName.ToLower() == field.ToLower())
                            {
                                propertyName = propName;
                                break;
                            }
                        }
                        #endregion

                        #region 构建表达式树
                        if (!string.IsNullOrWhiteSpace(propertyName))
                            wheres.Add(SimpleComparison<T>(entityType, propertyName, operate, value));
                        #endregion
                    }


                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }
            return wheres;
        }


        public static Expression<Func<TSource, bool>> SimpleComparison<TSource>
                    (Type type, string property, string operate, object value)
        {
            var pe = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(pe, property);
            var constantReference = Expression.Constant(value);
            BinaryExpression binaryExp = Expression.Equal(propertyReference, constantReference);
            switch (operate.ToLower())
            {
                case AppConst.S_QueryOperate_Eq:
                    binaryExp = Expression.Equal(propertyReference, constantReference);
                    break;
                case AppConst.S_QueryOperate_Like:

                    break;
                default:
                    Expression.Equal(propertyReference, constantReference);
                    break;
            }
            return Expression.Lambda<Func<TSource, bool>>(binaryExp, new[] { pe });
        }

    }
}

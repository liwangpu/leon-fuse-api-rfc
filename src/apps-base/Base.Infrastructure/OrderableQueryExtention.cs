using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Base.Infrastructure
{
    public static class OrderableQueryExtention
    {
        /// <summary>
        /// 使得Linq的OrderBy支持字符串的属性名，比如 _context.Items.OrderBy("Money").Take(10).ToList();
        /// https://stackoverflow.com/questions/31955025/generate-ef-orderby-expression-by-string
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertyName"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IEnumerable<TSource> query, string propertyName, bool desc = true)
        {
            var entityType = typeof(TSource);

            propertyName = string.IsNullOrWhiteSpace(propertyName) ? "id" : propertyName;

            var properties = entityType.GetProperties();
            var bFind = false;
            for (int idx = properties.Length - 1; idx >= 0; idx--)
            {
                var propName = properties[idx].Name.ToString();
                if (propName.ToLower() == propertyName.ToLower())
                {
                    propertyName = propName;
                    bFind = true;
                    break;
                }
            }//for

            if (!bFind)
                propertyName = "Id";




            //Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(Queryable);
            MethodInfo method = null;
            if (desc == true)
            {
                method = enumarableType.GetMethods()
                .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethodDefinition)
                .Where(m =>
                {
                    var parameters = m.GetParameters().ToList();
                    //Put more restriction here to ensure selecting the right overload                
                    return parameters.Count == 2;//overload that has 2 parameters
                }).Single();
            }
            else
            {
                method = enumarableType.GetMethods()
                 .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     //Put more restriction here to ensure selecting the right overload                
                     return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();
            }

            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Base.API.MiddleWares
{
    public class RequestCultureMiddleware
    {
        protected readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public virtual async Task InvokeAsync(HttpContext context)
        {
            //如果用户有自定义需求,优先使用自定义语言,否则从数据库查找用户默认语言
            //header[]和query[]是StringValues类型,永不为null,所以可以直接toString
            //另外,Headers[]key大小写不敏感
            var cultureHeader = context.Request.Headers["Accept-Language"].ToString().Trim().ToLower();
            var cultureQuery = context.Request.Query["culture"].ToString().Trim().ToLower();
            var culture = string.IsNullOrWhiteSpace(cultureQuery) ? cultureHeader : cultureQuery;

            if (!string.IsNullOrWhiteSpace(culture))
            {
                //校正简写
                culture = culture == "en" ? "en-US" : culture;
                culture = culture == "zh" ? "zh-Hans" : culture;
                context.Request.Headers["Accept-Language"] = culture;
                ////注意,这里直接设置CultureInfo没有用的
                ////起作用的方式还是直接将语言放到heads,让AcceptLanguageHeaderRequestCultureProvider起作用
                ////特此注释
                ////var ci = new CultureInfo(culture);
                ////CultureInfo.CurrentCulture = ci;
                ////CultureInfo.CurrentUICulture = ci;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}

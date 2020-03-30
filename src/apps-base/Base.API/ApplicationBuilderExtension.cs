using Base.API.MiddleWares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace Base.API
{
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// 配置默认语言支持
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
        {

            app.UseMiddleware<RequestCultureMiddleware>();
            var supportedCultures = new List<CultureInfo>
             {
               new CultureInfo("en-US"),
               new CultureInfo("zh-Hans"),
               new CultureInfo("zh-Hant")
               };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("zh-Hans"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            return app;
        }


    }
}

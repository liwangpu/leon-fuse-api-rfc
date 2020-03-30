using Base.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Web;

namespace Base.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPrifileContext(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IProfileContext>((provider) =>
            {
                var accessor = provider.GetService<IHttpContextAccessor>();
                var userIdStr = accessor?.HttpContext?.Request?.Headers["UserId"];
                var userNameStr = accessor?.HttpContext?.Request?.Headers["Username"];
                var tenantIdStr = accessor?.HttpContext?.Request?.Headers["TenantId"];
                var identityIdStr = accessor?.HttpContext?.Request?.Headers["IdentityId"];
                var uuidStr = accessor?.HttpContext?.Request?.Headers["UUID"];
                if (!string.IsNullOrWhiteSpace(userNameStr))
                {
                    userNameStr = HttpUtility.UrlDecode(userNameStr);
                }
                var context = new ProfileContext(identityIdStr, userIdStr, userNameStr, tenantIdStr, "");
                context.Properties.Add("UUID", uuidStr);
                return context;
            });

            return services;
        }

    }
}

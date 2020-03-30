using Base.API.ActionResults;
using Base.API.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace Base.API
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;
        private readonly IStringLocalizer<CommonTranslation> commonLocalizer;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger, IStringLocalizer<CommonTranslation> commonLocalizer)
        {
            this.env = env;
            this.logger = logger;
            this.commonLocalizer = commonLocalizer;
        }

        public void OnException(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();

            //400
            if (exceptionType == typeof(HttpBadRequestException))
            {
                var json = new AppJsonResponse
                {
                    Messages = new[] { context.Exception.Message }
                };
                context.Result = new ObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            //403
            else if (exceptionType == typeof(HttpForbiddenException))
            {
                var json = new AppJsonResponse
                {
                    Messages = new[] { commonLocalizer["PermissionDeny"].ToString() }
                };
                context.Result = new ObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            //404
            else if (exceptionType == typeof(HttpResourceNotFoundException))
            {
                var json = new AppJsonResponse
                {
                    Messages = new[] { context.Exception.Message }
                };
                context.Result = new ObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else if (exceptionType == typeof(HttpCustomizedException))
            {
                var ex = context.Exception as HttpCustomizedException;
                var json = new AppJsonResponse
                {
                    Messages = new[] { context.Exception.Message }
                };
                context.Result = new ObjectResult(json);
                context.HttpContext.Response.StatusCode = ex.HttpCode;
            }
            //自定义级别的异常
            //数据校验异常
            else if (context.Exception.InnerException != null && context.Exception.InnerException.GetType() == typeof(ValidationException))
            {
                var errors = context.Exception.InnerException as ValidationException;

                var json = new AppJsonResponse
                {
                    Messages = errors.Errors.Select(x => x.ErrorMessage).ToArray()
                };
                context.Result = new ObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            //其他
            else
            {
                var json = new AppJsonErrorResponse
                {
                    Messages = new[] { context.Exception.Message }
                };

                if (env.IsDevelopment())
                {
                    json.DeveloperMessage = context.Exception;
                }

                // Result asigned to a result object but in destiny the response is empty. This is a known bug of .net core 1.1
                // It will be fixed in .net core 1.1.2. See https://github.com/aspnet/Mvc/issues/5594 for more information
                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            logger.LogError(context.Exception, context.Exception.Message);

            context.ExceptionHandled = true;
        }
    }

    public class AppJsonResponse
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string[] Messages { get; set; }
    }

    public class AppJsonErrorResponse : AppJsonResponse
    {
        /// <summary>
        /// 开发调试信息
        /// </summary>
        public object DeveloperMessage { get; set; }
    }
}

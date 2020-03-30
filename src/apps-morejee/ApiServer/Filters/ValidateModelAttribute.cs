using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.Filters
{
    /// <summary>
    /// 模型绑定过滤器
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //context.Result = new BadRequestObjectResult(context.ModelState);
                context.Result = new ValidationFailedResult(context.ModelState);
            }
        }
    }

    /// <summary>
    /// Model校验返回结果
    /// </summary>
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState, int statusCode = StatusCodes.Status400BadRequest)
            : base(new ValidationResultModel(modelState))
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// ValidationError
    /// </summary>
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }

    /// <summary>
    /// ValidationResultModel
    /// </summary>
    public class ValidationResultModel
    {
        public string Message { get; }

        public List<ValidationError> Errors { get; }

        public ValidationResultModel(ModelStateDictionary modelState)
        {
            Message = "Validation Failed";
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                    .ToList();
        }
    }

    #region ErrorRespondModel 普通异常返回消息模版
    /// <summary>
    /// 普通异常返回消息模版
    /// </summary>
    public class ErrorRespondModel
    {
        public string Message { get; set; }
    }
    #endregion

}

using System;

namespace Base.API.Exceptions
{
    /// <summary>
    /// 错误的http请求异常,返回http 400
    /// </summary>
    public class HttpBadRequestException : Exception
    {
        public HttpBadRequestException()
        { }

        public HttpBadRequestException(string message)
            : base(message)
        { }

        public HttpBadRequestException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

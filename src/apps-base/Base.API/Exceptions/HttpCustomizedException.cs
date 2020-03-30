using System;

namespace Base.API.Exceptions
{
    public class HttpCustomizedException : Exception
    {
        public int HttpCode { get; protected set; }
        public HttpCustomizedException(int httpCode)
            : base()
        {
            HttpCode = httpCode;
        }

        public HttpCustomizedException(string message, int httpCode)
            : base(message)
        {
            HttpCode = httpCode;
        }

        public HttpCustomizedException(string message, Exception innerException, int httpCode)
            : base(message, innerException)
        {
            HttpCode = httpCode;
        }
    }
}

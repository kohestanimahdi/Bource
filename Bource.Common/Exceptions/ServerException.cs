using Bource.Common.Models;
using System;

namespace Bource.Common.Exceptions
{
    public class ServerException : AppException
    {
        public ServerException()
            : base(ApiResultStatusCode.ServerError)
        {
        }

        public ServerException(string message)
            : base(ApiResultStatusCode.ServerError, message)
        {
        }

        public ServerException(object additionalData)
            : base(ApiResultStatusCode.ServerError, additionalData)
        {
        }

        public ServerException(string message, object additionalData)
            : base(ApiResultStatusCode.ServerError, message, additionalData)
        {
        }

        public ServerException(string message, Exception exception)
            : base(ApiResultStatusCode.ServerError, message, exception)
        {
        }

        public ServerException(string message, Exception exception, object additionalData)
            : base(ApiResultStatusCode.ServerError, message, exception, additionalData)
        {
        }
    }
}

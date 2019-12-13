using System;
using System.Collections.Generic;
using System.Net;

namespace TriggerExceptionHandler
{
    public class Ext2HttpCode
    {
        private readonly IDictionary<Type, HttpStatusCode> _exceptionsCode = new Dictionary<Type, HttpStatusCode>
        {
            [typeof(UnauthorizedAccessException)] = HttpStatusCode.Unauthorized,
            [typeof(KeyNotFoundException)] = HttpStatusCode.NotFound,
        };

        public void Add<T>(HttpStatusCode statusCode) => Add(typeof(T), statusCode);

        public void Add(Type exceptionType, HttpStatusCode statusCode)
        {
            if (!exceptionType.IsSubclassOf(typeof(Exception)))
            {
                throw new TypeAccessException($"{nameof(exceptionType)} must derive from {nameof(Exception)}, {exceptionType} given");
            }

            _exceptionsCode[exceptionType] = statusCode;
        }

        public HttpStatusCode Get(Type type) => _exceptionsCode.ContainsKey(type) ? _exceptionsCode[type] : HttpStatusCode.InternalServerError;

        public HttpStatusCode Get(Exception exception) => Get(exception.GetType());

        public HttpStatusCode Get<T>() => Get(typeof(T));

        public bool Remove(Type type) => _exceptionsCode.Remove(type);

        public bool Remove<T>() => Remove(typeof(T));

        public bool Remove(Exception exception) => Remove(exception.GetType());
    }
}
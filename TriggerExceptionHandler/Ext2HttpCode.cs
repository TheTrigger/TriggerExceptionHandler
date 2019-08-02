﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace TriggerExceptionHandler
{
    public class Ext2HttpCode : IEnumerable<KeyValuePair<Type, HttpStatusCode>>
    {
        private static readonly IDictionary<Type, HttpStatusCode> _exceptionsCode = new Dictionary<Type, HttpStatusCode>()
        {
            {  typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized },
            {  typeof(KeyNotFoundException), HttpStatusCode.NotFound },
        };

        /// <summary>
        /// Retrieve <see cref="HttpStatusCode"/> from <see cref="Exception"/> type
        /// </summary>
        public HttpStatusCode this[Type exceptionType]
        {
            get
            {
                if (_exceptionsCode.ContainsKey(exceptionType))
                    return _exceptionsCode[exceptionType];

                return HttpStatusCode.InternalServerError;
            }

            set
            {
                if (!typeof(Exception).IsAssignableFrom(exceptionType))
                    throw new TypeAccessException($"{nameof(exceptionType)} must derive from {nameof(Exception)}, {exceptionType.GetType()} given");

                _exceptionsCode[exceptionType] = value;
            }
        }

        /// <summary>
        /// Retrieve <see cref="int"/> http status code from an <see cref="Exception"/> object
        /// </summary>
        public int this[Exception exception]
        {
            get
            {
                var t = exception.GetType();
                return (int)this[t];
            }
        }

        public void Add(Type exceptionType, HttpStatusCode statusCode)
        {
            this[exceptionType] = statusCode;
        }

        public IEnumerator<KeyValuePair<Type, HttpStatusCode>> GetEnumerator()
        {
            return _exceptionsCode.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace TriggerExceptionHandler.Demo.Exceptions
{
    [Serializable]
    internal class ExpectedException : Exception
    {
        public ExpectedException()
        {
        }

        public ExpectedException(string message) : base(message)
        {
        }

        public ExpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
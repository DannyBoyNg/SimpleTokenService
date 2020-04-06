using System;
using System.Runtime.Serialization;

namespace Ng.Services
{
    /// <summary>
    /// Token is expired Exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ExpiredTokenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiredTokenException"/> class.
        /// </summary>
        public ExpiredTokenException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiredTokenException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ExpiredTokenException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiredTokenException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public ExpiredTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiredTokenException"/> class.
        /// </summary>
        protected ExpiredTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
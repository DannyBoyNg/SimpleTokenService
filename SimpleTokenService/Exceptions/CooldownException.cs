using System;
using System.Runtime.Serialization;

namespace DannyBoyNg.Services
{
    /// <summary>
    /// Exception for violating the cooldown period
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class CooldownException : Exception
    {
        /// <summary>
        /// Gets or sets the cooldown time left.
        /// </summary>
        public TimeSpan? CooldownLeft { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownException"/> class.
        /// </summary>
        public CooldownException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CooldownException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public CooldownException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownException"/> class.
        /// </summary>
        protected CooldownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
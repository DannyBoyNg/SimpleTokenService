using System;

namespace DannyBoyNg.Services
{
    /// <summary>
    /// Default interface for simple token service
    /// </summary>
    public interface ISimpleTokenService
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        SimpleTokenSettings Settings { get; }

        /// <summary>
        /// Generates a simple token.
        /// </summary>
        /// <returns></returns>
        string GenerateSimpleToken();
        /// <summary>
        /// Uses a provided SimpleTokenRepository to store a simple token. If no SimpleTokenRepository is provided an exception will be thrown.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="simpleToken">The simple token.</param>
        void StoreToken(int userId, string simpleToken);
        /// <summary>
        /// Uses a provided SimpleTokenRepository to retrieve a simple token and validates the simple token. If no SimpleTokenRepository is provided an exception will be thrown.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="simpleToken">The simple token.</param>
        void ValidateToken(int userId, string simpleToken);
    }
}
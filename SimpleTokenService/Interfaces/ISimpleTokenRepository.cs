using System.Collections.Generic;

namespace Ng.Services
{
    /// <summary>
    /// Default interface for simple token repository
    /// </summary>
    public interface ISimpleTokenRepository
    {
        /// <summary>
        /// Deletes the specified simple token.
        /// </summary>
        /// <param name="simpleToken">The simple token.</param>
        void Delete(ISimpleToken simpleToken);
        /// <summary>
        /// Gets all the simple tokens by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        IEnumerable<ISimpleToken> GetByUserId(int userId);
        /// <summary>
        /// Inserts the simple token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="simpleToken">The simple token.</param>
        void Insert(int userId, string simpleToken);
    }
}
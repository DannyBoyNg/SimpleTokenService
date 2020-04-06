namespace Ng.Services
{
    /// <summary>
    /// Default interface for simple token
    /// </summary>
    public interface ISimpleToken
    {
        /// <summary>
        /// Gets or sets the simple token.
        /// </summary>
        string Token { get; set; }
        /// <summary>
        /// Gets or sets the user id that is associated with the simple token.
        /// </summary>
        int UserId { get; set; }
    }
}
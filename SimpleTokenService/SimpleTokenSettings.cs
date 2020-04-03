namespace DannyBoyNg.Services
{
    /// <summary>
    /// The simple token settings
    /// </summary>
    public class SimpleTokenSettings
    {
        /// <summary>
        /// Gets or sets the token expiration in minutes. default 1440 = 1 day
        /// </summary>
        public int TokenExpirationInMinutes { get; set; } = 1440;
        /// <summary>
        /// Gets or sets the cooldown period in minutes. A user may not create a new simpleToken within the cooldown period. This helps to prevent api misuse (like spamming)
        /// </summary>
        public int CooldownPeriodInMinutes { get; set; } = 5;
    }
}

using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Ng.Services
{
    /// <summary>
    /// The simple token service
    /// </summary>
    /// <seealso cref="Ng.Services.ISimpleTokenService" />
    public class SimpleTokenService : ISimpleTokenService
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public SimpleTokenSettings Settings { get; }
        ISimpleTokenRepository? SimpleTokenRepo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTokenService"/> class.
        /// </summary>
        /// <param name="simpleTokenRepo">The simple token repository.</param>
        public SimpleTokenService(ISimpleTokenRepository simpleTokenRepo)
        {
            Settings = new SimpleTokenSettings();
            SimpleTokenRepo = simpleTokenRepo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTokenService" /> class.
        /// </summary>
        /// <param name="simpleTokenRepo">The simple token repository.</param>
        /// <param name="options">The options.</param>
        public SimpleTokenService(
            ISimpleTokenRepository simpleTokenRepo,
            SimpleTokenSettings options)
        {
            Settings = options ?? new SimpleTokenSettings();
            SimpleTokenRepo = simpleTokenRepo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTokenService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="simpleTokenRepo">The simple token repository.</param>
        public SimpleTokenService(
            IOptions<SimpleTokenSettings> options,
            ISimpleTokenRepository? simpleTokenRepo = null)
        {
            Settings = options?.Value ?? new SimpleTokenSettings();
            SimpleTokenRepo = simpleTokenRepo;
        }

        /// <summary>
        /// Generates a simple token.
        /// </summary>
        public string GenerateSimpleToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// Uses a provided SimpleTokenRepository to retrieve a simple token and validates the simple token. If no SimpleTokenRepository is provided an exception will be thrown.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="simpleToken">The simple token.</param>
        /// <exception cref="ExpiredTokenException">Thrown when simple token is expired</exception>
        /// <exception cref="InvalidTokenException">Thrown when does not exist in the data store</exception>
        public void ValidateToken(int userId, string simpleToken)
        {
            if (SimpleTokenRepo == null) throw new NoSimpleTokenRepositorySetException();
            var tokenExpired = false;
            var tokens = SimpleTokenRepo.GetByUserId(userId);
            //Remove expired simple tokens from db
            foreach (var token in tokens)
            {
                if (IsExpired(token.Token))
                {
                    if (token.Token == simpleToken) tokenExpired = true;
                    SimpleTokenRepo.Delete(token);
                }
            }
            //Validate user provided simple token
            var dbToken = tokens.Where(x => x.Token == simpleToken).SingleOrDefault();
            if (dbToken != null) SimpleTokenRepo.Delete(dbToken);
            if (tokenExpired) throw new ExpiredTokenException();
            if (dbToken == null) throw new InvalidTokenException();
        }

        /// <summary>
        /// Uses a provided SimpleTokenRepository to store a simple token. If no SimpleTokenRepository is provided an exception will be thrown.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="simpleToken">The simple token.</param>
        /// <exception cref="CooldownException">Thrown when storing a simple token during the cooldown period.</exception>
        public void StoreToken(int userId, string simpleToken)
        {
            if (SimpleTokenRepo == null) throw new NoSimpleTokenRepositorySetException();
            if (InCooldownPeriod(userId, out TimeSpan? cooldownLeft)) throw new CooldownException($"You must wait at least {Settings.CooldownPeriodInMinutes} minutes to perform this action again") { CooldownLeft = cooldownLeft };
            SimpleTokenRepo.Insert(userId, simpleToken);
        }

        private static DateTime GetCreationTime(string simpleToken)
        {
            if (simpleToken == null) throw new ArgumentNullException(nameof(simpleToken));
            simpleToken = simpleToken.Replace('_', '/').Replace('-', '+');
            switch (simpleToken.Length % 4)
            {
                case 2: simpleToken += "=="; break;
                case 3: simpleToken += "="; break;
            }
            byte[] data = Convert.FromBase64String(simpleToken);
            return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        }

        private bool InCooldownPeriod(int userId, out TimeSpan? cooldownLeft)
        {
            var token = GetMostRecent(userId);
            if (token == null)
            {
                cooldownLeft = null;
                return false;
            }
            var tokenCreationTime = GetCreationTime(token.Token);
            cooldownLeft = tokenCreationTime - DateTime.UtcNow.AddMinutes(-1 * Settings.CooldownPeriodInMinutes);
            return tokenCreationTime > DateTime.UtcNow.AddMinutes(-1 * Settings.CooldownPeriodInMinutes);
        }

        private ISimpleToken? GetMostRecent(int userId)
        {
            if (SimpleTokenRepo == null) throw new NoSimpleTokenRepositorySetException();
            DateTime? timestamp = null;
            ISimpleToken? mostRecent = null;
            var tokens = SimpleTokenRepo.GetByUserId(userId).ToList();
            foreach (var token in tokens)
            {
                if (IsExpired(token.Token))
                {
                    SimpleTokenRepo.Delete(token);
                }
                var creation = GetCreationTime(token.Token);
                if (timestamp == null || creation > timestamp)
                {
                    timestamp = creation;
                    mostRecent = token;
                }
            }
            return mostRecent;
        }

        private bool IsExpired(string simpleToken)
        {
            if (Settings.TokenExpirationInMinutes == 0) return false; //When set to 0, simple token never expires
            DateTime when = GetCreationTime(simpleToken);
            return when < DateTime.UtcNow.AddMinutes(Settings.TokenExpirationInMinutes * -1);
        }
    }
}

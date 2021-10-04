# SimpleTokenService

A service to create and validate simple tokens in C#.

## Dependancies

Microsoft.Extensions.Options

## Installing

Install from Nuget
```
Install-Package Ng.SimpleTokenService
```

## Usage

**Console application**

```csharp
using Ng.Services;
...
//Example implementation of an inMemory repository for refresh tokens. In production, you would use a database store and not an inMemory store.
ISimpleTokenRepository inMemoryRepository = new MyInMemorySimpleTokenRepository(); //Never use this in production

//SimpleToken Settings
var settings = new SimpleTokenSettings
{
    CooldownPeriodInMinutes = 5, //Default: 5
    TokenExpirationInMinutes = 1440, //Default: 1440 (1 Day). Set to 0 to never expire
};

//Providing your own implementation of a simpleTokenRepository is required. Settings are not required if you want to use defaults.
var simpleTokenService = new SimpleTokenService(inMemoryRepository, settings); 

//Generate a simple token
string simpleToken = simpleTokenService.GenerateSimpleToken();

//Example User data
var userId = 1;

//Store a simple token. Throws CooldownException
simpleTokenService.StoreToken(userId, simpleToken);

//Validate a simple token (Does this token belong to this user and is it still valid). Throws ExpiredTokenException and InvalidTokenException
simpleTokenService.ValidateToken(userId, simpleToken);
```

Example of an inMemory simpleToken repository (never use in production). Use a database store instead.

```csharp
public class MyInMemorySimpleTokenRepository : ISimpleTokenRepository
{
    static readonly List<ISimpleToken> inMemStore = new List<ISimpleToken>();

    public void Delete(ISimpleToken refreshToken)
    {
        var item = inMemStore.Where(x => x.Token == refreshToken.Token && x.UserId == refreshToken.UserId).SingleOrDefault();
        if (item != null) inMemStore.Remove(item);
    }

    public void DeleteAll()
    {
        inMemStore.Clear();
    }

    public IEnumerable<ISimpleToken> GetByUserId(int userId)
    {
        return inMemStore.Where(x => x.UserId == userId);
    }

    public void Insert(int userId, string refreshToken)
    {
        inMemStore.Add(new SimpleToken { UserId = userId, Token = refreshToken });
    }
}

public class SimpleToken : ISimpleToken
{
    public string Token { get; set; }
    public int UserId { get; set; }
}
```

**ASP.NET Core**

Register service with dependency injection in Startup.cs
```csharp
using Ng.Services;
...
public void ConfigureServices(IServiceCollection services)
{
    //Settings are not required if you want to use defaults.
    services.AddSimpleTokenService(options => {
        options.CooldownPeriodInMinutes = 5; //Default: 5
        options.TokenExpirationInMinutes = 1440; //Default: 1440 (1 Day). Set to 0 to never expire
    });
    //Make sure you provide you own implementation of ISimpleTokenRepository. Replace MyInMemorySimpleTokenRepository with your own repository.
    services.AddScoped<ISimpleTokenRepository, MyInMemorySimpleTokenRepository>();
}
```

Inject ISimpleTokenService into a Controller or wherever you like
```csharp
using Ng.Services;
...
public class MyController
{
    public MyController(ISimpleTokenService simpleTokenService) // <-- Inject ISimpleTokenService here
    {
        //You can use simpleTokenService now
        
    }
}
```
**More examples**

An example using a database (this example was provided by user @faina09)
```csharp
    internal class MySqlSimpleTokenRepository : ISimpleTokenRepository
    {
        protected readonly DbContext _myDbContext;
        private DbSet<SimpleSqlToken> SimpleTokenDbSet { get; set; }

        public MySqlSimpleTokenRepository()
        {
            string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=MyDbName;Integrated Security=True";
            var options = GetOptions(connectionString);
            _myDbContext = new MyDbContext(options);
            SimpleTokenDbSet = _myDbContext.Set<SimpleSqlToken>();
        }

        public DbContextOptions<MyDbContext> GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<MyDbContext>(), connectionString).Options;
        }

        public void Delete(ISimpleToken simpleToken)
        {
            var item = SimpleTokenDbSet.Where(x => x.Token == simpleToken.Token && x.UserId == simpleToken.UserId).SingleOrDefault();
            if (item != null) SimpleTokenDbSet.Remove(item);
            _myDbContext.SaveChanges();
        }

        public IEnumerable<ISimpleToken> GetByUserId(int userId)
        {
            return SimpleTokenDbSet.Where(x => x.UserId == userId);
        }

        public void Insert(int userId, string simpleToken)
        {
            SimpleTokenDbSet.Add(new SimpleSqlToken { UserId = userId, Token = simpleToken });
            _myDbContext.SaveChanges();
        }
    }

    public partial class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public virtual DbSet<SimpleSqlToken> SimpleSqlToken { get; set; }
    }

    [Table("SimpleToken")]
    public class SimpleSqlToken : ISimpleToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
    }
```

Table was created with
```sql
CREATE TABLE [dbo].[SimpleToken](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Token] [varchar](250) NULL,
 CONSTRAINT [PK_SimpleToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
```

## License

This project is licensed under the MIT License.

## Contributions

Contributions are welcome.

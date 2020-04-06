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

Console application

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

ASP.NET Core

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

## License

This project is licensed under the MIT License.

## Contributions

Contributions are welcome.

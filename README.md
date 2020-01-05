# Dahomey.Cbor.AspNetCore
Asp.Net Core Support for [Dahomey.Cbor](https://github.com/dahomey-technologies/Dahomey.Cbor)

![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Dahomey.Cbor.AspNetCore)
![](https://github.com/dahomey-technologies/Dahomey.Cbor.AspNetCore/workflows/Build%20and%20Test/badge.svg)

## Features
* Asp.net core 2.1, 2.2, 3.0 and 3.1 CBOR formatters

## Setup
You can enable [Dahomey.Cbor](https://github.com/dahomey-technologies/Dahomey.Cbor) as a CBOR formatter in ASP.NET Core 2.1 or 2.2 by using the Nuget package [Dahomey.Cbor.AspNetCore](https://www.nuget.org/packages/Dahomey.Cbor.AspNetCore/). To enable it, add the extension method ``AddDahomeyCbor(CborOptions options = null)`` to the ``AddMvc()/AddControllers()`` call in ``ConfigureServices``

### Asp.net core 2.x
```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    CborOptions options = new CborOptions();
    services.AddMvc()
      .AddDahomeyCbor(options);
}
```

### Asp.net core 3.x
```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    CborOptions options = new CborOptions();
    services.AddControllers()
      .AddDahomeyCbor(options);
}
```

### HTTP Headers
If an incoming HTTP request holds the following headers:
* ``Content-Type`` with the value ``application/cbor``: the Request body will be deserilized in CBOR.
* ``Accept`` with the value ``application/cbor``: the Response body will be serialized in CBOR.

If the headers are missing, the default JSON formatters will be used.

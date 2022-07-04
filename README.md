**/ ! \ Warning**  
**This project is not finished and should not be used in production, it does not meet the minimum security requirements.**

# Secure instant messaging

This repository contains the Secure Instant Messaging **student work of HE2B ESI SECG4 lab**.

## Report

[Rapport.pdf](https://github.com/neod/SECG4-InstantMessaging/blob/master/Rapport.pdf)

## Dependencies
This application work with .NET 5 framework, Entity Framework core tools and MariaDB/MySQL server (SQLite for dev only).

### To install .NET 5 SDK with SNAP

```
sudo snap install dotnet-sdk --classic --channel=5.0
```

### To install EF core tools

```
dotnet tool install --global dotnet-ef --version 5.0.6
```

### To configure Database (MariaDB or MySQL)
You can modify the connection string in `/instantMessagingServer/instantMessagingServer/appsettings.json`

```
"sql": "server=[ADDRESS];user=[USER];password=[PASSWORD];database=[DATABASE]"
``` 

### Nuget package (installed with make command)
InstantMessagingServer:

- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/6.1.4?_src=template)
- [Microsoft.VisualStudio.Web.CodeGeneration.Design](https://www.nuget.org/packages/Microsoft.VisualStudio.Web.CodeGeneration.Design/5.0.2?_src=template)
- [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/5.0.6?_src=template)
- [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/5.0.6?_src=template)
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/5.0.6?_src=template)
- [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/5.0.0?_src=template)

InstantMessagingClient:

- [SimpleTCP.Core](https://www.nuget.org/packages/SimpleTCP.Core/1.0.4?_src=template)
- [RestSharp](https://www.nuget.org/packages/RestSharp/106.11.7?_src=template)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/13.0.1?_src=template)
- [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/5.0.6?_src=template)
- [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/5.0.6?_src=template)
- [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/5.0.0?_src=template)
- [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/5.0.0?_src=template)
- [EasyConsoleApplication](https://www.nuget.org/packages/EasyConsoleApplication/0.4.0?_src=template)

## Build process
InstantMessagingServer:

run `make` in /instantMessagingServer directory

InstantMessagingClient:

run `make` in /instantMessagingClient directory

## Usage
### InstantMessagingServer:

Launche the server with
```
dotnet run --project instantMessagingServer/instantMessagingServer
```

You can configure token key, token validity duration and connection string in
`/instantMessagingServer/instantMessagingServer/appsettings.json`

### InstantMessagingClient:

You have to configure the serveur adresse and port in `/instantMessagingClient/instantMessagingClient/config.json`

Launche the client with
```
dotnet run --project instantMessagingClient/instantMessagingClient
```

## Authors
- **54024 Arno Pierre Pion**
- **54456 Damiano Deplano**

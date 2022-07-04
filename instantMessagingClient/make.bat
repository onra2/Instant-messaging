dotnet restore
dotnet build
dotnet ef database update --project instantMessagingServer
cp instantMessagingClient\instantMessaging.db instantMessagingClient\bin\Debug\net5.0
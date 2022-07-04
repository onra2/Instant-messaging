#NOM    : Makefile
#CLASSE : SECG4
#OBJET  : instantMessagingClient
#HOWTO	: make
#AUTEUR	: 54456 Damiano Deplano, 05/2021

Init: databaseUpdate
	cp instantMessagingClient/instantMessaging.db instantMessagingClient/bin/Debug/net5.0

databaseUpdate: Build
	dotnet ef database update --project instantMessagingClient

Build: RestoreNUGET
	dotnet build 

RestoreNUGET:
	dotnet restore
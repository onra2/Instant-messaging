#NOM    : Makefile
#CLASSE : SECG4
#OBJET  : instantMessagingServer
#HOWTO	: make
#AUTEUR	: 54456 Damiano Deplano, 05/2021

Init: Build
	dotnet ef database update --project instantMessagingServer

Build: RestoreNUGET
	dotnet build 

RestoreNUGET:
	dotnet restore
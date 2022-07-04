using System;
using System.IO;
using System.Net;
using System.Security;
using EasyConsoleApplication;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Database;
using instantMessagingClient.Model;
using instantMessagingCore.Models.Dto;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;

namespace instantMessagingClient.Pages
{
    public class LoginPage : Page
    {
        public LoginPage()
        {
            ConsoleHelpers.Write(ConsoleColor.Green, "Login");
            ConsoleHelpers.Write(ConsoleColor.White, "-----");
            ConsoleHelpers.Write(ConsoleColor.White, "Enter your username and password to login: ");

            Rest rest = new Rest();
            IRestResponse response;

            do
            {
                //ask for username, pass until the response is correct
                string username;
                SecureString password;
                do
                {
                    username = ConsoleHelpers.Readline(ConsoleColor.White, "Username: ");
                    password = Program.getPasswordFromConsole("Password: ");
                    if (username == null)
                    {
                        ConsoleHelpers.WriteRed("\nUsername/Password can't be empty");
                    }
                } while (username == null);
                Console.WriteLine();

                //API call to login, if successfull get the token and put it in our session
                response = rest.Login(username, password);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    ConsoleHelpers.WriteRed("There was an error, make sure you registered or that your name and password are correct.");
                    var key = ConsoleHelpers.AskToUserYesNoQuestion(ConsoleColor.Yellow, "Go back to the home page?");
                    Console.WriteLine();
                    if (key.Key == ConsoleKey.N)
                    {
                        continue;
                    }
                    Application.GoTo<Home>();
                }
                var responseContent = response.Content;
                Tokens deserializeObject = JsonConvert.DeserializeObject<Tokens>(responseContent);
                Session.tokens = deserializeObject;
                Session.sessionPassword = password;
                Session.sessionUsername = username;
                DatabaseContext db = new DatabaseContext();
                db.Database.EnsureCreated();
                var myPrivateKey = db.MyKey.FirstOrDefault(k => k.UserId == Session.tokens.UserId);//Get our private key in session
                Session.maKey = myPrivateKey;

                ConsoleHelpers.WriteGreen("Successfully logged in " + username + "!");
                Console.WriteLine();
            }
            while (response.StatusCode != HttpStatusCode.OK);

            ConsoleHelpers.HitEnterToContinue();
            Application.GoTo<LoggedInHomePage>();
        }
    }
}

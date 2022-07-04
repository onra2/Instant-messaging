using System;
using System.Net;
using System.Security;
using EasyConsoleApplication;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Database;
using instantMessagingClient.Model;
using instantMessagingCore.Crypto;
using instantMessagingCore.Models.Dto;
using Newtonsoft.Json;
using RestSharp;

namespace instantMessagingClient.Pages
{
    public class RegisterPage : Page
    {
        /// <summary>
        /// Page where the registration happens
        /// </summary>
        public RegisterPage()
        {
            ConsoleHelpers.Write(ConsoleColor.Yellow, "Register");
            ConsoleHelpers.Write(ConsoleColor.White, "-----");

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

                //make a request to register
                response = rest.Inscription(username, password);
                //if the registration failed, ask again
                if (response is {IsSuccessful: false})
                {
                    ConsoleHelpers.WriteRed(response.Content.Contains("PASSWORD_POLICY")
                        ? response.Content
                        : "There was an error, make sure the username doesn't already exists, the password isn't empty.");

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ConsoleHelpers.WriteRed(response.Content);
                    }
                    var key = ConsoleHelpers.AskToUserYesNoQuestion(ConsoleColor.Yellow, "Go back to the home page?");
                    Console.WriteLine();
                    if (key.Key == ConsoleKey.N)
                    {
                        continue;
                    }
                    Application.GoTo<Home>();
                }

                //if the registration was successfull put the received token in our session
                if (response != null)
                {
                    var responseContent = response.Content;
                    Tokens deserializeObject = JsonConvert.DeserializeObject<Tokens>(responseContent);
                    Session.tokens = deserializeObject;
                }

                Session.sessionPassword = password;
                Session.sessionUsername = username;

                //Generate a new private and public key, store the public key on the server and store the private one in our local
                //database
                RSAManager myKeys = new RSAManager();
                IRestResponse publicKeyResponse = rest.postKey(myKeys.GetKey(false));
                if (publicKeyResponse is {IsSuccessful: true})
                {
                    var myPrivateKey = myKeys.GetKey(true);
                    DatabaseContext db = new DatabaseContext();
                    db.Database.EnsureCreated();
                    MyKey key = new MyKey(Session.tokens.UserId, myPrivateKey);
                    db.MyKey.Add(key);
                    db.SaveChanges();
                    ConsoleHelpers.WriteGreen("Successfully registered " + username + "!");
                }
            }
            while (response != null && response.StatusCode != HttpStatusCode.OK);

            ConsoleHelpers.HitEnterToContinue();
            Application.GoTo<LoginPage>();
        }
    }
}

using System;
using EasyConsoleApplication;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Model;
using RestSharp;

namespace instantMessagingClient.Pages
{
    public class AddFriend : Page
    {
        public AddFriend()
        {
            ConsoleHelpers.WriteGreen("If you want to go back, type '/back'");
            const string backCommand = "/back";
            string Name;
            bool nameExists;
            Rest rest = new Rest();
            do
            {
                Name = ConsoleHelpers.Readline(ConsoleColor.White, "name: ");
                if (Name == backCommand)
                {
                    nameExists = true;
                    continue;
                }
                IRestResponse rep = rest.SendFriendRequest(Name);
                if (rep != null)
                {
                    nameExists = rep.IsSuccessful;
                    if (nameExists)
                    {
                        ConsoleHelpers.WriteGreen("Successfully added " + Name);
                        ConsoleHelpers.HitEnterToContinue();
                    }
                    else
                    {
                        ConsoleHelpers.WriteRed(Name + " doesn't exist.");
                    }
                }
                else
                {
                    nameExists = false;
                }
            } while (Name != backCommand && !nameExists);

            Application.GoTo<FriendList>();
        }
    }
}

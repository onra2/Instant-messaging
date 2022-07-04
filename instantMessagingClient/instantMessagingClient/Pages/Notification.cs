using System;
using EasyConsoleApplication;
using EasyConsoleApplication.Menus;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Model;
using instantMessagingCore.Models.Dto;

namespace instantMessagingClient.Pages
{
    public class Notification : Page
    {
        private static int _ID;
        private static string name;
        public Notification(int ID, string friendName)
        {
            _ID = ID;//user ID that wants to be friend
            name = friendName;

            Title = name;
            TitleColor = ConsoleColor.Green;
            Body = "-----";
            MenuItems.Add(new MenuItem("Accept", Accept));
            MenuItems.Add(new MenuItem("Decline", Decline){ Color = ConsoleColor.Red});//TODO: block
            MenuItems.Add(new MenuItem("Go back", Application.GoBack)
            {
                Color = ConsoleColor.Yellow
            });
        }

        /// <summary>
        /// Accept the friend request
        /// </summary>
        private static void Accept()
        {
            Rest rest = new Rest();
            var reply = rest.ActionFriendRequest(Friends.Action.accept, _ID);
            if (reply != null)
            {
                if (reply.IsSuccessful)
                {
                    ConsoleHelpers.WriteGreen("Successfully added " + name);
                    ConsoleHelpers.HitEnterToContinue();
                }
                else
                {
                    ConsoleHelpers.WriteRed("Error while adding friend :(");
                    Console.WriteLine(reply.Content);
                    ConsoleHelpers.HitEnterToContinue();
                }
            }
            else
            {
                ConsoleHelpers.WriteRed("Error while adding friend :(");
                ConsoleHelpers.HitEnterToContinue();
            }

            Application.GoTo<FriendList>();
        }

        /// <summary>
        /// Decline the friend request
        /// </summary>
        private static void Decline()
        {
            ConsoleKeyInfo yesOrNo = ConsoleHelpers.AskToUserYesNoQuestion(ConsoleColor.Red, "Are you sure about that?");
            Console.WriteLine();
            if (yesOrNo.Key == ConsoleKey.Y)
            {
                Rest rest = new Rest();
                var reply = rest.ActionFriendRequest(Friends.Action.refuse, _ID);
                if (reply != null)
                {
                    if (reply.IsSuccessful)
                    {
                        ConsoleHelpers.WriteGreen("Successfully declined " + name);
                        ConsoleHelpers.HitEnterToContinue();
                    }
                    else
                    {
                        ConsoleHelpers.WriteRed("Error while declining request.");
                        ConsoleHelpers.HitEnterToContinue();
                    }
                }
                Application.GoTo<FriendList>();
            }
        }
    }
}

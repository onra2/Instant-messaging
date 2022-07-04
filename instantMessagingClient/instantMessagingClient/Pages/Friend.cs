using System;
using EasyConsoleApplication;
using EasyConsoleApplication.Menus;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Model;
using instantMessagingCore.Models.Dto;

namespace instantMessagingClient.Pages
{
    public class Friend : Page
    {
        private static int _ID;

        public Friend(int ID, string friendName)
        {
            _ID = ID;

            Title = friendName;
            TitleColor = ConsoleColor.Green;
            Body = "-----";
            MenuItems.Add(new MenuItem("Message", () => Application.GoTo<MessageFriend>(_ID, friendName)));
            MenuItems.Add(new MenuItem("Delete from friendlist", DeleteFriend)
            {
                Color = ConsoleColor.Red
            });
            MenuItems.Add(new MenuItem("Go back", Application.GoBack)
            {
                Color = ConsoleColor.Yellow
            });
        }

        /// <summary>
        /// Delete the specific friend
        /// </summary>
        private static void DeleteFriend()
        {
            ConsoleKeyInfo yesOrNo = ConsoleHelpers.AskToUserYesNoQuestion(ConsoleColor.Red, "Are you sure about that?");
            if (yesOrNo.Key == ConsoleKey.Y)
            {
                Rest rest = new Rest();
                var reply = rest.ActionFriendRequest(Friends.Action.refuse, _ID);
                Application.GoTo<FriendList>();
            }
        }
    }
}
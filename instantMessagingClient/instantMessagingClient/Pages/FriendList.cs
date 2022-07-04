using System;
using EasyConsoleApplication;
using EasyConsoleApplication.Menus;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Model;
using instantMessagingCore.Models.Dto;
using Newtonsoft.Json;

namespace instantMessagingClient.Pages
{
    public class FriendList : Page
    {
        public FriendList()
        {
            Title = Session.sessionUsername + "(" + Session.tokens.UserId + ")'" + "Friends list";
            TitleColor = ConsoleColor.Green;
            Body = "-----";
            PrintFriendsAndNotifications();
            MenuItems.Add(Separator.Instance);
            MenuItems.Add(new MenuItem("Add friend", () => Application.GoTo<AddFriend>()));
            MenuItems.Add(new MenuItem("Go back", () => Application.GoTo<LoggedInHomePage>())
            {
                Color = ConsoleColor.Yellow
            });
        }

        /// <summary>
        /// Prints our friends and all incoming friend requests
        /// </summary>
        private void PrintFriendsAndNotifications()
        {
            Rest rest = new Rest();

            //for amis
            var reponseMyFriends = rest.getMyFriendList();
            if (reponseMyFriends is {IsSuccessful: true})
            {
                Friends[] FriendList = JsonConvert.DeserializeObject<Friends[]>(reponseMyFriends.Content);
                if (FriendList != null)
                {
                    foreach (Friends friend in FriendList)
                    {
                        var friendUserId = friend.UserId == Session.tokens.UserId ? friend.FriendId : friend.UserId;
                        var rep = rest.getUserById(friendUserId);
                        MenuItems.Add(new MenuItem(rep.Content, () => Application.GoTo<Friend>(friendUserId, rep.Content)));
                    }
                }
            }
            
            //for notif
            var reponseFriendRequests = rest.getFriendRequests();
            if (reponseFriendRequests != null)
            {
                if (reponseFriendRequests.IsSuccessful)
                {
                    Friends[] FriendList = JsonConvert.DeserializeObject<Friends[]>(reponseFriendRequests.Content);
                    if (FriendList != null)
                    {
                        foreach (Friends friend in FriendList)
                        {
                            var rep = rest.getUserById(friend.UserId);
                            MenuItems.Add(new MenuItem(rep.Content + " wants to add you.",
                                () => Application.GoTo<Notification>(friend.UserId, rep.Content))
                            {
                                Color = ConsoleColor.Green
                            });
                        }
                    }
                }
            }
        }
    }
}

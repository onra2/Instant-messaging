using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyConsoleApplication;
using EasyConsoleApplication.Menus;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Model;

namespace instantMessagingClient.Pages
{
    public class Home : Page
    {
        //Home page of the program
        public Home()
        {
            Title = "Instant messaging";
            TitleColor = ConsoleColor.Green;
            Body = "-----";
            MenuItems.Add(new MenuItem("Login", () => Application.GoTo<LoginPage>())
            {
                Color = ConsoleColor.Green
            });
            MenuItems.Add(new MenuItem("Register", () => Application.GoTo<RegisterPage>())
            {
                Color = ConsoleColor.Yellow
            });
            MenuItems.Add(Separator.Instance);
            MenuItems.Add(new MenuItem("Quit", Application.Exit));
        }
    }
}

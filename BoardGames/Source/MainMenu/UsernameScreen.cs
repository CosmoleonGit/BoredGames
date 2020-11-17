using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;

namespace BoardGames.Source.MainMenu
{
    class UsernameScreen : MenuScreen
    {
        public UsernameScreen(Action action)
        {
            components.Add(new Label(this)
            {
                Position = new Point(Main.screenWidth / 2, Main.screenHeight / 4),
                Font = Main.MediumFont,
                Text = "Enter a username.",
                TextAlign = Label.TextAlignEnum.CENTRE_CENTRE
            });

            components.Add(new Label(this)
            {
                Position = new Point(Main.screenWidth / 2 - 25, Main.screenHeight / 2),
                Font = Main.MediumFont,
                Text = "Username:",
                TextAlign = Label.TextAlignEnum.CENTRE_RIGHT
            });

            TextBox usernameTxt = new TextBox(this, Main.main.Window)
            {
                Position = new Point(Main.screenWidth / 2 + 25, Main.screenHeight / 2 - 10),
                Size = new Point(100, 20),
                charLimit = 15,
                Font = Main.SmallFont,
                Text = Properties.Settings.Default.username
            };

            Button button = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50, Main.screenHeight * 3 / 4 - 25),
                Font = Main.MediumFont,
                Text = "OK",
                Size = new Point(100, 50)
            };

            button.ClickEvent += (object s) =>
            {
                void Error(string title)
                {
                    Main.main.Screen = new OKScreen(title, new Action(() =>
                    {
                        Main.main.Screen = new UsernameScreen(action);
                    }));
                }

                if (usernameTxt.Text == "")
                {
                    Error("You must enter a username.");
                } 
                else if (usernameTxt.Text.Length > 20)
                {
                    Error("That username is too long.");
                } else
                {
                    Properties.Settings.Default.username = usernameTxt.Text;
                    Properties.Settings.Default.Save();

                    //main.screen = new ChooseScreen(main);
                    action();
                }
            };

            usernameTxt.EnterPressed += () =>
            {
                button.PerformClick();
            };

            components.Add(button);
            components.Add(usernameTxt);
        }
    }
}

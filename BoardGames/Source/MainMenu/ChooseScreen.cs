using Microsoft.Xna.Framework;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.MainMenu
{
    public class ChooseScreen : MenuScreen
    {
        public ChooseScreen()
        {
            Button serverButton = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50 - 75, Main.screenHeight / 2 - 25),
                Size = new Point(100, 50),
                Font = Main.MediumFont,
                Text = "Host Game",
                buttonColour = Color.DarkRed,
                hoverColour = Color.Red,
                textColour = Color.White
            };

            serverButton.ClickEvent += (object s) =>
            {
                Main.main.Screen = new HostScreen();
            };

            Button clientButton = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50 + 75, Main.screenHeight / 2 - 25),
                Size = new Point(100, 50),
                Font = Main.MediumFont,
                Text = "Join Game",
                buttonColour = Color.Green,
                hoverColour = Color.Lime,
                textColour = Color.White
            };

            clientButton.ClickEvent += (object s) =>
            {
                Main.main.Screen = new JoinScreen();
            };

            Button settingsButton = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 75, Main.screenHeight * 3/4),
                Size = new Point(150, 50),
                Font = Main.MediumFont,
                Text = "Change Username"
            };

            settingsButton.ClickEvent += (object s) =>
            {
                Main.main.Screen = new UsernameScreen(new Action(() => Main.main.Screen = new ChooseScreen()));
            };

            components.Add(serverButton);
            components.Add(clientButton);
            components.Add(settingsButton);
        }
    }
}

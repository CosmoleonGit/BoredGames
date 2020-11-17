using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;

namespace BoardGames.Source.MainMenu
{
    public class OKScreen : MenuScreen
    {
        public OKScreen(string title, Action onClick)
        {
            components.Add(new Label(this)
            {
                Position = new Point(Main.screenWidth / 2, Main.screenHeight / 4),
                Font = Main.MediumFont,
                Text = title,
                TextAlign = Label.TextAlignEnum.CENTRE_CENTRE
            });

            Button button = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50, Main.screenHeight * 3 / 4 - 25),
                Font = Main.MediumFont,
                Text = "OK",
                Size = new Point(100, 50)
            };

            button.ClickEvent += (object s) => onClick();

            components.Add(button);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;

namespace BoardGames.Source.Games.SplashScreens
{
    public class OptionsOverlay : OverlayScreen
    {

        public OptionsOverlay(string message, string[] options)
        {
            Label label = new Label(this)
            {
                Position = new Point(BoredGame.wh / 2, BoredGame.wh / 4),
                Font = Main.MediumFont,
                Text = message,
                TextAlign = Label.TextAlignEnum.CENTRE_CENTRE
            };

            components.Add(label);

            for (int i = 0; i < options.Length; i++)
            {
                Button button = new Button(this)
                {
                    Position = new Point((BoredGame.wh / 2) - (100 / 2), 
                                         (BoredGame.wh / 2) - (options.Length / 2 * 50) + i * 50),
                    Size = new Point(100, 50),
                    Font = Main.MediumFont,
                    Text = options[i]
                };

                button.ClickEvent += Button_ClickEvent;

                components.Add(button);
            }
            
        }

        public Action<string> action;

        private void Button_ClickEvent(object sender)
        {
            action?.Invoke((sender as Button).Text);
            TakeControl = false;
        }
    }
}

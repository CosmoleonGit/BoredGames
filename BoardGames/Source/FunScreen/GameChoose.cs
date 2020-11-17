using BoardGames.Source.Games;
using BoardGames.Source.Games.TurnGames.Chess;
using BoardGames.Source.Games.TurnGames.Draughts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;
using BoardGames.Source.Games.TurnGames.Shogi;
using BoardGames.Source.Games.TurnGames.HasamiShogi;
using MonoNet;

namespace BoardGames.Source.FunScreen
{
    internal class GameChoose : ControlGroup
    {
        static readonly string[] options = { "Chess", "Draughts", "Hasami Shogi", "Connect Four", "Dots and Boxes", "T-Rex Runner"};

        readonly MainScreen mainScreen;
        public GameChoose(MainScreen _mainScreen)
        {
            mainScreen = _mainScreen;

            Label label = new Label(this)
            {
                Position = new Point(BoredGame.wh / 2, BoredGame.wh / 4),
                Font = Main.MediumFont,
                Text = "Select a game to play.",
                TextAlign = Label.TextAlignEnum.CENTRE_CENTRE
            };

            components.Add(label);

            for (int i = 0; i < options.Length; i++)
            {
                Button button = new Button(this)
                {
                    Position = new Point((BoredGame.wh / 2) - (150 / 2),
                                         (BoredGame.wh / 2) - (options.Length / 2 * 50) + i * 50),
                    Size = new Point(150, 50),
                    Font = Main.MediumFont,
                    Text = options[i]
                };
                
                button.ClickEvent += ButtonClick;

                components.Add(button);
            }

        }

        private void ButtonClick(object s)
        {
            string value = (s as Button).Text;

            bool white = Main.random.NextDouble() < 0.5;
            //Console.WriteLine(white);

            var msg = Networking.CreateMessage();
            msg.Write((byte)MainScreen.Side.GAME_SIDE);
            msg.Write(value + "," + (white ? "B" : "W"));
            Networking.SendMessage(msg);

            mainScreen.LoadFromString(value, white);
        }
    }
}

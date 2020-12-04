using BoardGames.Source.Games;
using BoardGames.Source.Games.TurnGames.Chess;
using BoardGames.Source.Games.TurnGames.Draughts;
using BoardGames.Source.Games.TurnGames.HasamiShogi;
using BoardGames.Source.Games.TurnGames.Shogi;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using MonoExt;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoNet;

namespace BoardGames.Source.FunScreen
{
    internal class WaitForChoose : ControlGroup, ICanReceive
    {
        readonly MainScreen mainScreen;

        public WaitForChoose(MainScreen _mainScreen)
        {
            mainScreen = _mainScreen;

            components.Add(new Label(this)
            {
                Position = new Point(BoredGame.wh / 2, BoredGame.wh / 2),
                Font = Main.MediumFont,
                Text = "Waiting for the host to choose a game to play...",
                TextAlign = Label.TextAlignEnum.CENTRE_CENTRE
            });
        }

        public void ReceiveMessage(NetIncomingMessage msg)
        {
            /*
            string[] parts = msg.ReadString().Split(',');

            if (parts.Length != 2) return;

            bool white = parts[1] == "W";

            mainScreen.LoadFromString(parts[0], white);
            */

            string n = msg.ReadString();
            int seed = msg.ReadInt32();

            mainScreen.LoadFromString(n, seed);
        }
    }
}

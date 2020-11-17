using BoardGames.Source.FunScreen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;
using Lidgren.Network;
using MonoNet;

namespace BoardGames.Source.Games.SplashScreens
{
    class WaitingOverlay : OverlayScreen, ICanReceive
    {
        public WaitingOverlay(string message)
        {
            components.Add(new Label(this)
            {
                Position = new Point(BoredGame.wh / 2, BoredGame.wh / 2),
                Font = Main.MediumFont,
                Text = message,
                TextAlign = Label.TextAlignEnum.CENTRE_CENTRE
            });
        }

        public void ReceiveMessage(NetIncomingMessage msg)
        {

            action(msg.ReadBoolean());
            TakeControl = false;
        }

        public Action<bool> action;
    }
}

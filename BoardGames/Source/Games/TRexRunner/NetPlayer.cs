using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TRexRunner
{
    public class NetPlayer : Sprite, ICanReceive
    {
        public override Texture2D GetTexture => Player.tex;

        enum MoveType
        {
            POS,
            DUCK
        }

        public void ReceiveMessage(NetIncomingMessage message)
        {
            switch ((MoveType)message.ReadByte())
            {
                case MoveType.POS:
                    position.Y = message.ReadFloat();
                    break;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames.FourInARow
{
    internal class Counter : Piece
    {
        public static Texture2D red, yellow;

        static Counter()
        {
            red = Main.main.Content.Load<Texture2D>("ConnectFour/Red");
            yellow = Main.main.Content.Load<Texture2D>("ConnectFour/Yellow");
        }

        public Counter(bool white) : base(white) { }

        protected override Texture2D GetWhiteTex() => red;
        protected override Texture2D GetBlackTex() => yellow;
    }
}

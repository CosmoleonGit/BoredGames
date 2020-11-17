using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames
{
    public abstract class Piece
    {
        public Piece(bool _white) => white = _white;

        protected abstract Texture2D GetWhiteTex();
        protected abstract Texture2D GetBlackTex();

        public Texture2D GetTexture => white ? GetWhiteTex() : GetBlackTex();

        public bool white;

        public void Show(SpriteBatch spriteBatch, Rectangle r)
        {
            Texture2D tex = GetTexture;
            if (tex != null)
                spriteBatch.Draw(tex, r, Color.White);
        }
    }
}

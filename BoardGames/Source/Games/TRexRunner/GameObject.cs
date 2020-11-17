using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TRexRunner
{
    public abstract class GameObject : Component
    {
        public Vector2 position;
        public Vector2 size;

        public RectangleF Rectangle => new RectangleF(position, size);

        public bool CollidingWith(GameObject other) => RectangleF.Intersect(Rectangle, other.Rectangle);

        protected Color color = Color.White;

        protected abstract Texture2D GetTexture();

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetTexture(), new Rectangle(position.ToPoint(), size.ToPoint()), color);
        }
    }
}

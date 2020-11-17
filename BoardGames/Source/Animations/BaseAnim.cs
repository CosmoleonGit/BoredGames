using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using System;

namespace BoardGames.Source.Animations
{
    public abstract class BaseAnim : Component
    {
        public BaseAnim(Vector2 pos, Action finishAction)
        {
            //board = _board;
            size = Vector2.One;
            position = pos;
            afterFinish = finishAction;
        }

        protected Vector2 position;

        protected abstract Texture2D GetTexture();

        private bool finished;
        public bool Finished
        {
            get { return finished; }
            protected set
            {
                finished = value;
                if (value) { OnFinish(); afterFinish?.Invoke(); };
            }
        }

        private readonly Action afterFinish;

        protected virtual void OnFinish() { }

        protected Vector2 size;

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D tex = GetTexture();

            //spriteBatch.Draw(GetTexture(), new Rectangle(position.ToPoint(), size), Color.White);
            spriteBatch.Draw(tex,
                             position,
                             null,
                             Color.White,
                             0f,
                             new Vector2(tex.Width / 2, tex.Height / 2),
                             size / new Vector2(tex.Width, tex.Height),
                             SpriteEffects.None,
                             0f);
        }
    }
}

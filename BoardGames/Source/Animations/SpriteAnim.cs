using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BoardGames.Source.Animations
{
    public class SpriteAnim : BaseAnim
    {
        public SpriteAnim(Texture2D[] _frames, Vector2 pos, Action finishAction) : base(pos, finishAction) { frames = _frames; }

        readonly Texture2D[] frames;

        int i;
        const int frameDuration = 5;

        public override void Update(GameTime gameTime)
        {
            i++;
            if (i == frameDuration * frames.Length) Finished = true;
        }

        protected override Texture2D GetTexture()
        {
            return frames[i / frameDuration];
        }
    }
}

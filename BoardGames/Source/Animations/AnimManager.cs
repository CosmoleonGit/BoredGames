using BoardGames.Source.Games.TurnGames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Animations
{
    public class AnimManager : Component
    {
        readonly List<BaseAnim> animations = new List<BaseAnim>();

        public void Add(BaseAnim anim) =>
            animations.Add(anim);

        public int Count => animations.Count;

        public override void Update(GameTime gameTime)
        {
            for (int i = animations.Count - 1; i >= 0; i--)
            {
                animations[i].Update(gameTime);

                if (animations[i].Finished)
                    animations.RemoveAt(i);
            }
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].Show(gameTime, spriteBatch);
            }
        }
    }
}

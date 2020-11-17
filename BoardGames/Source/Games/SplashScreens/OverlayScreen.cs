using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.SplashScreens
{
    public abstract class OverlayScreen : ControlGroup
    {
        private static readonly Color transparent = new Color(0, 0, 0, 0);
        private static readonly Color backColour = new Color(0, 0, 0, 200);

        private Color currentColour = transparent;

        float l = 0f;
        const float speed = 0.025f;

        public bool TakeControl { get; protected set; } = true;

        //protected List<Component> components = new List<Component>();

        public bool Finished { get; private set; }

        public override void Update(GameTime gameTime)
        {
            if (TakeControl)
            {
                //components.ForEach(x => x.Update(gameTime));
                base.Update(gameTime);

                if (l != 1)
                {
                    l += speed;
                    if (l > 1) l = 1;

                    currentColour = Color.Lerp(transparent, backColour, l);
                }
            } else
            {
                if (l != 0)
                {
                    l -= speed;
                    if (l <= 0)
                    {
                        l = 0;
                        Finished = true;
                    } else { currentColour = Color.Lerp(transparent, backColour, l); }
                }
            }
            
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(SpecialContent.Pixel, new Rectangle(0, 0, BoredGame.wh, BoredGame.wh), currentColour);
            spriteBatch.End();
            //if (TakeControl) components.ForEach(x => x.Show(gameTime, spriteBatch));
            if (TakeControl) base.Show(gameTime, spriteBatch);
        }
    }
}

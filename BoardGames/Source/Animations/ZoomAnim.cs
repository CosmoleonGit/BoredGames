using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BoardGames.Source.Animations
{
    public class ZoomAnim : BaseAnim
    {
        public ZoomAnim(Texture2D tex, Vector2 pos, Action finishAction, Vector2 _bSize, Vector2 _eSize, int _delay = 0) : base(pos, finishAction)
        {
            position = pos + Vector2.One / 2;

            //start = pos + eSize / 2;
            //end = pos + bSize / 2;
            texture = tex;
            size = _bSize;
            bSize = _bSize;
            eSize = _eSize;
            delay = _delay;
        }

        //Vector2 start;
        //Vector2 end;

        Vector2 bSize;
        Vector2 eSize;

        readonly Texture2D texture;

        float l = 0;
        const float speed = 0.04f;

        int delay;

        public override void Update(GameTime gameTime)
        {
            if (delay == 0)
            {
                l += speed;
                if (l >= 1) Finished = true;

                //position = Vector2.SmoothStep(start, end, l);
                size = Vector2.SmoothStep(bSize, eSize, l);
            }
            else
            {
                delay--;
            }
        }

        protected override Texture2D GetTexture()
        {
            return texture;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BoardGames.Source.Animations
{
    public class MoveAnim : BaseAnim
    {

        public MoveAnim(Texture2D _tex, Vector2 _start, Vector2 _dest, Action finishAction) : base(_start, finishAction)
        {
            start = _start;
            dest = _dest;
            tex = _tex;
        }

        Vector2 start;
        Vector2 dest;

        float l = 0;
        const float speed = 1f / 30;

        //readonly Piece piece;

        readonly Texture2D tex;

        protected override void OnFinish()
        {
            //Point destPos = dest.ToPoint();

            //board.pieces[destPos.X, destPos.Y] = piece;
        }

        public override void Update(GameTime gameTime)
        {
            l += speed;
            position = Vector2.SmoothStep(start, dest, l);

            if (l >= 1) Finished = true;
        }

        protected override Texture2D GetTexture() => tex;
    }
}

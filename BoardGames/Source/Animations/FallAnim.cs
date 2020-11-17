using BoardGames.Source.Games.TurnGames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Animations
{
    public class FallAnim : BaseAnim
    {
        public FallAnim(Texture2D _tex, Vector2 pos, Action finishAction, Action _bounceAction, float _gravity, float _maxVel, int _bounces, float _mul, float _floor) : base(pos, finishAction)
        {
            tex = _tex;
            bounceAction = _bounceAction;
            maxVel = _maxVel;
            bounces = _bounces;
            mul = _mul;
            gravity = _gravity;
            floor = _floor;
        }

        float vel;

        readonly Texture2D tex;
        readonly float maxVel, gravity, floor, mul;

        readonly Action bounceAction;

        int bounces;

        protected override Texture2D GetTexture() => tex;

        public override void Update(GameTime gameTime)
        {
            vel += gravity;
            vel = Math.Min(vel, maxVel);

            position.Y += vel;

            if (position.Y > floor)
            {
                bounces--;
                if (bounces > 0)
                {
                    vel *= -mul;
                } else
                {
                    Finished = true;
                }

                position.Y = floor;
                bounceAction?.Invoke();
            }
        }
    }
}

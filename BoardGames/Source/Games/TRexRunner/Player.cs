using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TRexRunner
{
    public class Player : GameObject
    {
        public Player()
        {
            color = Color.Red;
            size = Vector2.One * 32;
        }

        public static Texture2D tex;
        static Player()
        {
            //tex = Main.main.Content.Load<Texture2D>("");
            tex = SpecialContent.Pixel;
        }

        float yVelocity;
        const float gravity = 0.35f;
        const float terminalVel = 15f;

        const float jumpHeight = 8f;

        protected override Texture2D GetTexture()
        {
            return tex;
        }

        bool grounded;

        public override void Update(GameTime gameTime)
        {
            if (grounded && Input.KeyPressed(Keys.Space) || Input.KeyPressed(Keys.W) || Input.KeyPressed(Keys.Up))
            {
                yVelocity = -jumpHeight;
            }

            grounded = false;

            yVelocity += gravity;
            yVelocity = Math.Min(yVelocity, terminalVel);

            position.Y += yVelocity;
            if (position.Y > TRexRunnerGame.groundHeight - size.Y)
            {
                grounded = true;
                position.Y = Math.Min(position.Y, TRexRunnerGame.groundHeight - size.Y);
            }
            
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Show(gameTime, spriteBatch);
        }
    }
}

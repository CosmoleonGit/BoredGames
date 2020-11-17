using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.MainMenu
{
    public class PlayScreen : MenuScreen
    {
        static Texture2D logo;
        static PlayScreen()
        {
            logo = Main.main.Content.Load<Texture2D>("Logo");
            logoRect = new Rectangle(Main.screenWidth / 2 - (int)(logo.Width * 1.5), Main.screenHeight / 16, logo.Width * 3, logo.Height * 3);
        }
        
        /*
        public static void LoadContent(ContentManager content)
        {
            logo = content.Load<Texture2D>("Logo");
        }
        */

        public PlayScreen()
        {
            Button playButton = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50, Main.screenHeight * 3/4 - 25),
                Size = new Point(100, 50),
                Font = Main.MediumFont,
                Text = "Play",
            };

            playButton.ClickEvent += (object s) =>
            {
                Main.main.Screen = new ChooseScreen();
            };

            components.Add(playButton);
        }

        static readonly Rectangle logoRect;

        float l = 0f;

        public override void Update(GameTime gameTime)
        {
            l += 1f;
            if (l > 360f) l -= 360f;

            base.Update(gameTime);
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Show(gameTime, spriteBatch);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //spriteBatch.Draw(logo, new Rectangle(0, 0, logoRect.Width, logoRect.Height), null, Color.White, 1f, logoRect.Center.ToVector2(), SpriteEffects.None, 0f);
            spriteBatch.Draw(logo, new Vector2(Main.screenWidth / 2, Main.screenHeight / 4), null, Color.White, (float)Math.Sin(MathHelper.ToRadians(l)) / 6, new Vector2(logoRect.Width / 6, logoRect.Height / 6), Vector2.One * 3, SpriteEffects.None, 0f);
            spriteBatch.End();
            //spriteBatch.Draw(logo, new Rectangle(Main.screenWidth / 2 - (int)(logo.Width * 1.5), Main.screenHeight / 16, logo.Width * 3, logo.Height * 3), null, Color.White, 5f, new Vector2(Main.screenWidth / 2, Main.screenHeight / 16), SpriteEffects.None, 0f);
        }
    }
}

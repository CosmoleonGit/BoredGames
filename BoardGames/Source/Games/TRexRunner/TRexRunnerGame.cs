using BoardGames.Source.FunScreen;
using BoardGames.Source.Games.TRexRunner;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games
{
    public class TRexRunnerGame : BoredGame
    {
        public TRexRunnerGame(MainScreen mainScreen) : base(mainScreen)
        {
            player = new Player();
        }

        public const int groundHeight = Main.screenHeight / 2 - 50;

        Player player;

        public override void AfterUpdate(GameTime gameTime)
        {
            player.Update(gameTime);
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            player.Show(gameTime, spriteBatch);
            spriteBatch.End();
        }

        protected override void RelayMessage2(NetIncomingMessage msg)
        {
            
        }
    }
}

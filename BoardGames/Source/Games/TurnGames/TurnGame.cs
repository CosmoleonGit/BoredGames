using BoardGames.Source.FunScreen;
using BoardGames.Source.Games.SplashScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;
using MonoNet;
using Lidgren.Network;

namespace BoardGames.Source.Games
{
    public abstract class TurnGame : BoredGame, ICanReceive
    {
        

        protected Label label;

        protected virtual string WhiteStr => "white";
        protected virtual string BlackStr => "black";

        public TurnGame(MainScreen _mainScreen, int seed) : base(_mainScreen, seed)
        {

            playerWhite = seed % 2 == (Networking.IsServer ? 0 : 1);
            if (!ColourFirst) playerWhite ^= true;

            label = new Label(this)
            {
                Font = Main.MediumFont,
                //Text = "You are " + (white ? WhiteStr : BlackStr) + ".",
                TextAlign = Label.TextAlignEnum.CENTRE_LEFT,
                Position = new Point(20, 537)
            };

            ResetLabel();

            components.Add(label);
        }
        
        internal bool turn;
        internal bool playerWhite = true;

        protected virtual bool ColourFirst => true;

        //protected enum Colour { WHITE, BLACK}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Show(gameTime, spriteBatch);
        }

        public void SwapTurn()
        {
            turn ^= true;
            ResetLabel();
        }

        protected override void ResetGame()
        {
            ResetLabel();
        }

        private void ResetLabel()
        {
            label.Text = "You are " + (playerWhite ? WhiteStr : BlackStr) + ".";
            if (turn == playerWhite) label.Text += " (Your Turn)";
        }
    }
}

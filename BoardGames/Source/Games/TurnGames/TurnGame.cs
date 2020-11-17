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

        public TurnGame(MainScreen _mainScreen, bool white) : base(_mainScreen)
        {
            playerWhite = white;

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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Show(gameTime, spriteBatch);
        }

        protected virtual void ResetGame() { ResetLabel(); }

        public void GameEnd(string message)
        {
            if (Networking.IsServer)
            {
                var yesNo = new OptionsOverlay(message + Environment.NewLine + Environment.NewLine + "Play again?", new string[] { "Yes", "No" });
                yesNo.action += (string s) =>
                {
                    var msg = Networking.CreateMessage();
                    msg.Write((byte)MainScreen.Side.GAME_SIDE);

                    if (s == "Yes")
                    {
                        msg.Write(true);

                        ResetGame();
                    } else
                    {
                        msg.Write(false);
                        
                        mainScreen.gameSide = new GameChoose(mainScreen);
                    }

                    Networking.SendMessage(msg);
                };
                overlay = yesNo;
            } else
            {
                var waiting = new WaitingOverlay(message + Environment.NewLine + Environment.NewLine + "Waiting for host to make a decision...");

                waiting.action += (bool s) =>
                {
                    if (s)
                    {
                        ResetGame();
                        //overlay = null;
                    } else
                    {
                        mainScreen.gameSide = new WaitForChoose(mainScreen);
                    }
                };

                overlay = waiting;
            }
        }

        public void SwapTurn()
        {
            turn ^= true;
            ResetLabel();
        }

        private void ResetLabel()
        {
            label.Text = "You are " + (playerWhite ? WhiteStr : BlackStr) + ".";
            if (turn == playerWhite) label.Text += " (Your Turn)";
        }
    }
}

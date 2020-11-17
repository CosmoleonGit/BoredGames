using BoardGames.Source.FunScreen;
using BoardGames.Source.Games.SplashScreens;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games
{
    public abstract class BoredGame : ControlGroup, ICanReceive
    {
        internal OverlayScreen overlay;

        protected MainScreen mainScreen;

        public BoredGame(MainScreen _mainScreen)
        {
            mainScreen = _mainScreen;

            Button resignBtn = new Button(this)
            {
                Position = new Point(412, 512),
                Size = new Point(100, 50),
                Font = Main.MediumFont,
                Text = "Quit Game"
            };

            resignBtn.ClickEvent += (object s) =>
            {
                resignBtn.active = false;

                var msg = Networking.CreateMessage();
                msg.Write((byte)MainScreen.Side.GAME_SIDE);
                msg.Write((byte)MessageType.RESIGN);
                Networking.SendMessage(msg);

                var wait = new WaitingOverlay("You requested to quit the game. Waiting for a response...");

                wait.action += (bool quit) =>
                {
                    if (quit)
                    {
                        if (Networking.IsServer)
                        {
                            mainScreen.gameSide = new GameChoose(mainScreen);
                        }
                        else
                        {
                            mainScreen.gameSide = new WaitForChoose(mainScreen);
                        }
                    }
                    else
                    {
                        resignBtn.active = true;
                    }
                };

                overlay = wait;
            };

            components.Add(resignBtn);
        }

        public const int wh = 512;

        protected enum MessageType
        {
            GAME,
            RESIGN
        }

        public static NetOutgoingMessage CreateGameMessage()
        {
            var msg = Networking.CreateMessage();
            
            msg.Write((byte)MainScreen.Side.GAME_SIDE);
            msg.Write((byte)MessageType.GAME);

            return msg;
        }

        public void ReceiveMessage(NetIncomingMessage msg)
        {
            if (overlay != null && overlay.TakeControl && overlay is ICanReceive ics)
            {
                ics.ReceiveMessage(msg);
            } else
            {
                var messageType = (MessageType)msg.ReadByte();

                if (messageType == MessageType.RESIGN)
                {
                    var optionOverlay = new OptionsOverlay("Your opponent requested to quit the game." + Environment.NewLine + Environment.NewLine + "Do you accept?", new string[] { "Yes", "No" });
                    optionOverlay.action += (string ret) =>
                    {
                        var newMsg = Networking.CreateMessage();
                        newMsg.Write((byte)MainScreen.Side.GAME_SIDE);

                        if (ret == "Yes")
                        {
                            newMsg.Write(true);

                            if (Networking.IsServer)
                            {
                                mainScreen.gameSide = new GameChoose(mainScreen);
                            }
                            else
                            {
                                mainScreen.gameSide = new WaitForChoose(mainScreen);
                            }
                        }
                        else
                        {
                            newMsg.Write(false);
                        }


                        Networking.SendMessage(newMsg);
                    };

                    overlay = optionOverlay;
                }
                else 
                {
                    RelayMessage2(msg);
                }
            }
            
        }

        protected abstract void RelayMessage2(NetIncomingMessage msg);

        public override void Update(GameTime gameTime)
        {
            if (overlay != null)
            {
                overlay.Update(gameTime);
                if (overlay.Finished) overlay = null;
            }

            if (overlay == null || !overlay.TakeControl)
            {
                base.Update(gameTime);

                AfterUpdate(gameTime);
            }
        }

        public abstract void AfterUpdate(GameTime gameTime);

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (overlay != null)
            {
                overlay.Show(gameTime, spriteBatch);
            }
            else { base.Show(gameTime, spriteBatch); }
            //base.Show(gameTime, spriteBatch);
        }
    }
}

using BoardGames.Source.Games;
using BoardGames.Source.Games.TurnGames.ConnectFour;
using BoardGames.Source.Games.TurnGames.Chess;
using BoardGames.Source.Games.TurnGames.Draughts;
using BoardGames.Source.Games.TurnGames.HasamiShogi;
using BoardGames.Source.Games.TurnGames.Shogi;
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
using BoardGames.Source.Games.TurnGames.DotsAndBoxes;

namespace BoardGames.Source.FunScreen
{
    public class MainScreen : ControlGroup, ICanReceive
    {
        //public readonly Main main;

        public MainScreen()
        {
            //main = _main;
            //gameSide = new DraughtsGame(true);
            if (Networking.IsServer)
            {
                gameSide = new GameChoose(this);
            } else
            {
                gameSide = new WaitForChoose(this);
            }

            chat = new ChatScreen();
        }

        public Component gameSide;
        readonly ChatScreen chat;

        public override void Update(GameTime gameTime)
        {
            gameSide.Update(gameTime);
            chat.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gameSide.Show(gameTime, spriteBatch);
            chat.Show(gameTime, spriteBatch);
            base.Show(gameTime, spriteBatch);
        }

        public enum Side
        {
            GAME_SIDE,
            CHAT_SIDE
        }

        public void ReceiveMessage(NetIncomingMessage msg)
        {
            switch ((Side)msg.ReadByte())
            {
                case Side.GAME_SIDE:
                    if (gameSide is ICanReceive gs) gs.ReceiveMessage(msg);
                    break;
                    
                case Side.CHAT_SIDE:
                    chat.ReceiveMessage(msg);
                    break;
            }
        }

        internal void LoadFromString(string s, bool white)
        {
            switch (s)
            {
                case "Chess":
                    gameSide = new ChessGame(this, white);
                    break;
                case "Draughts":
                    gameSide = new DraughtsGame(this, white);
                    break;
                case "Shogi":
                    gameSide = new ShogiGame(this, white);
                    break;
                case "Hasami Shogi":
                    gameSide = new HasamiShogiGame(this, white);
                    break;
                case "Connect Four":
                    gameSide = new ConnectFourGame(this, white);
                    break;
                case "Dots and Boxes":
                    gameSide = new DotsAndBoxesGame(this, white);
                    break;
                case "T-Rex Runner":
                    gameSide = new TRexRunnerGame(this);
                    break;
            }
        }
    }
}

using BoardGames.Source.FunScreen;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoExt;
using MonoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.BlockFall
{
    public partial class BlockFallGame : BoredGame
    {
        const int scl = 24;

        BlockFallInstance left, right;

        static readonly Matrix leftMat, rightMat;

        static BlockFallGame()
        {
            leftMat = Matrix.CreateScale(scl) *
                      Matrix.CreateTranslation(new Vector3(wh / 4 - BlockFallInstance.w * scl / 2, 
                                                           wh / 2 - BlockFallInstance.h * scl / 2, 0));
            rightMat = Matrix.CreateScale(scl) *
                       Matrix.CreateTranslation(new Vector3(wh / 4 * 3 - BlockFallInstance.w * scl / 2,
                                                                wh / 2 - BlockFallInstance.h * scl / 2, 0));
        }

        internal Random serverRnd, clientRnd;

        public BlockFallGame(MainScreen mainScreen, int seed) : base(mainScreen, seed)
        {
            serverRnd = new Random(seed);

            unchecked
            {
                clientRnd = new Random(seed + int.MaxValue);
            }

            ResetGame();
        }

        protected override void ResetGame()
        {
            left = new BlockFallInstance(this, serverRnd, true);
            right = new BlockFallInstance(this, clientRnd, false);

            PlayerSide.Reset();
        }

        public override void AfterUpdate(GameTime gameTime)
        {
            PlayerSide.Update(gameTime);
            
        }

        BlockFallInstance PlayerSide =>
            Networking.IsServer ? left : right;

        BlockFallInstance OtherSide =>
            Networking.IsServer ? right : left;

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: leftMat);
            left.Show(gameTime, spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: rightMat);
            right.Show(gameTime, spriteBatch);
            spriteBatch.End();

            base.Show(gameTime, spriteBatch);
        }

        enum TetrisMsgType
        {
            DROP,
            MOVE_RIGHT,
            MOVE_LEFT,
            ANTICLOCKWISE_ROT,
            CLOCKWISE_ROT
        }

        protected override void RelayMessage2(NetIncomingMessage msg)
        {
            switch ((TetrisMsgType)msg.ReadByte())
            {
                case TetrisMsgType.DROP:
                    OtherSide.Fall();
                    break;
                case TetrisMsgType.MOVE_LEFT:
                    OtherSide.Move(-1);
                    break;
                case TetrisMsgType.MOVE_RIGHT:
                    OtherSide.Move(1);
                    break;
                case TetrisMsgType.CLOCKWISE_ROT:
                    OtherSide.RotateClockwise();
                    break;
                case TetrisMsgType.ANTICLOCKWISE_ROT:
                    OtherSide.RotateAnticlockwise();
                    break;
            }
        }
    }
}

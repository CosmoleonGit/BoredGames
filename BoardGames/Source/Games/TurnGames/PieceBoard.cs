using BoardGames.Source.Animations;
using BoardGames.Source.FunScreen;
using BoardGames.Source.Games.SplashScreens;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames
{
    public abstract partial class PieceBoard : TurnGame, ICanReceive
    {
        protected static SoundEffect moveSound;
        
        static PieceBoard()
        {
            moveSound = Main.main.Content.Load<SoundEffect>("Chess Piece");
        }

        /*
        public static void LoadContent(ContentManager content)
        {
            moveSound = content.Load<SoundEffect>("Chess Piece");
        }
        */
        

        bool doNotUpdate;

        readonly int d;

        public PieceBoard(MainScreen mainScreen, bool white, int _d) : base(mainScreen, white)
        {
            d = _d;
            scl = wh / d;
            boardMatrix = Matrix.CreateScale(scl);
            boardMatrix *= Matrix.CreateTranslation(new Vector3((wh % d) / 2, (wh % d) / 2, 0f));

            pieces = new MobilePiece[d, d];
            selectedTiles = new bool[d, d];
            animations = new AnimManager();

            ResetGame();
        }

        public override void AfterUpdate(GameTime gameTime)
        {
            //if (overlay != null && overlay.TakeControl) { }
            if (animations.Count > 0)
            {
                animations.Update(gameTime);
                /*
                for (int i = animations.Count - 1; i >= 0; i--)
                {
                    animations[i].Update(gameTime);
                }

                animations.RemoveAll(x => x.Finished);
                */

                if (animations.Count == 0)
                {
                    if (!doNotUpdate)
                    {
                        AfterMove();
                    } else { doNotUpdate = false; }
                    
                }
            }
            else
            {

                if (Input.LeftMousePressed())
                {
                    Point mousePos = Input.MousePositionMatrix(boardMatrix).ToPoint();

                    if (InBounds(mousePos.X, mousePos.Y))
                    {
                        //selectedTiles[mousePos.X, mousePos.Y] ^= true;

                        if (selectedTiles[mousePos.X, mousePos.Y])
                        {
                            selectedTiles = new bool[d, d];

                            /*
                            pieces[selectedPiece.X, selectedPiece.Y].BeforeMove
                                (this, selectedPiece.X, selectedPiece.Y, mousePos.X, mousePos.Y);
                            */

                            pieces[selectedPiece.X, selectedPiece.Y].Move
                                (this, selectedPiece.X, selectedPiece.Y, mousePos.X, mousePos.Y, new Action(() => 
                                pieces[selectedPiece.X, selectedPiece.Y].BeforeMove(this, selectedPiece.X, selectedPiece.Y, mousePos.X, mousePos.Y)));
                            
                            
                            //SendMessage(selectedPiece.X + "," + selectedPiece.Y + "," + mousePos.X + "," + mousePos.Y);
                        }
                        else
                        {
                            MobilePiece piece = pieces[mousePos.X, mousePos.Y];

                            if (piece != null)
                            {
                                selectedTiles = piece.PossibleMoves(this, mousePos.X, mousePos.Y);
                                selectedPiece = mousePos;
                            }
                            else
                            {
                                selectedTiles = new bool[d, d];
                            }
                        }
                    }
                }
            }
        }

        internal MobilePiece[,] pieces;

        private Point selectedPiece;
        //private Selection[,] selectedTiles;
        private bool[,] selectedTiles;

        internal bool InBounds(int x, int y) =>
            x >= 0 && x < d && y >= 0 && y < d;

        protected override void ResetGame()
        {
            LoopThroughAll((x, y) =>
            {
                pieces[x, y] = null;
            });

            doNotUpdate = true;
            base.ResetGame();
            //ResetLabel();
        }

        protected bool CanMove(bool whiteSide)
        {
            bool anyPossible(bool[,] possible)
            {
                for (int x = 0; x < d; x++)
                {
                    for (int y = 0; y < d; y++)
                    {
                        if (possible[x, y]) return true;
                    }
                }

                return false;
            };

            for (int x = 0; x < d; x++)
            {
                for (int y = 0; y < d; y++)
                {
                    if (pieces[x, y] != null && pieces[x, y].white == whiteSide && anyPossible(pieces[x, y].PossibleMoves2(this, x, y)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected int ColourCount(bool whiteSide)
        {
            int num = 0;

            LoopThroughAll((x, y) =>
            {
                if (pieces[x, y] != null && pieces[x, y].white == whiteSide)
                {
                    num++;
                }
            });

            return num;
        }

        protected bool ColourCountThreshold(bool whiteSide, Func<int, bool> func)
        {
            int num = 0;
            for (int x = 0; x < d; x++)
            {
                for (int y = 0; y < d; y++)
                {
                    if (pieces[x, y] != null && pieces[x, y].white == whiteSide)
                    {
                        num++;
                        if (func(num)) return true;
                    }
                }
            }

            return false;
        }

        protected void LoopThroughAll(Action<int, int> action)
        {
            for (int x = 0; x < d; x++)
            {
                for (int y = 0; y < d; y++)
                {
                    action(x, y);
                }
            }
        }

        internal bool ComparePieceColours(MobilePiece p1, MobilePiece p2) => 
            p1 != null && p2 != null &&
            p1.white == p2.white;

        internal void SetPiece(MobilePiece piece, int x, int y)
        {
            //animations.Add(new ZoomAnim(piece.GetTexture, new Vector2(x, y), new Action(() => pieces[x, y] = piece), Vector2.Zero, Vector2.One, (x + y) * 2));
            animations.Add(new ZoomAnim(piece.GetTexture, new Vector2(x, y), new Action(() => pieces[x, y] = piece), Vector2.Zero, Vector2.One, (x + y) * 2));
            //pieces[x, y] = piece;
        }
        protected virtual void AfterMove()
        {
            //ResetLabel();
        }

        public readonly int scl = 64;
        public readonly Matrix boardMatrix;

        //internal List<BaseAnim> animations;
        internal AnimManager animations;

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix:boardMatrix);

            LoopThroughAll((x, y) =>
            {
                Rectangle r = new Rectangle(x, y, 1, 1);

                // Draw background
                Color c;
                if (selectedTiles[x, y])
                {
                    c = (pieces[x, y] == null) ? Color.Cyan : Color.Red;
                }
                else
                {
                    c = (x + y) % 2 == 0 ? Color.Beige : Color.Orange;
                }

                spriteBatch.Draw(SpecialContent.Pixel, r, c);


                // Draw piece
                pieces[x, y]?.Show(spriteBatch, r);

                // Draw animations
                //animations.ForEach(anim => anim.Show(gameTime, spriteBatch));
                animations.Show(gameTime, spriteBatch);
            });

            spriteBatch.End();

            base.Show(gameTime, spriteBatch);
        }

        protected override void RelayMessage2(NetIncomingMessage msg)
        {
            int x1 = msg.ReadInt32(),
                y1 = msg.ReadInt32(),
                x2 = msg.ReadInt32(),
                y2 = msg.ReadInt32();

            pieces[x1, y1]?.Move(this, x1, y1, x2, y2);
        }
    }
}

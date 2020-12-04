using BoardGames.Source.Animations;
using BoardGames.Source.FunScreen;
using BoardGames.Source.Games.TurnGames.FourInARow;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames.FourInARow
{
    public class FourInARowGame : TurnGame
    {
        static Texture2D box, redArrow, yellowArrow;

        static SoundEffect bounceSound;

        protected override string WhiteStr => "red";
        protected override string BlackStr => "yellow";

        static FourInARowGame()
        {
            box = Main.main.Content.Load<Texture2D>("ConnectFour/Box");

            redArrow = Main.main.Content.Load<Texture2D>("ConnectFour/Red Arrow");
            yellowArrow = Main.main.Content.Load<Texture2D>("ConnectFour/Yellow Arrow");

            bounceSound = Main.main.Content.Load<SoundEffect>("ConnectFour/ConnectFourCounter");

            matrix = Matrix.CreateScale(scl) * 
                     Matrix.CreateTranslation(new Vector3(wh / 2 - w * scl / 2, wh / 2 - h * scl / 2, 0f));
        }

        static Matrix matrix;

        const int scl = 64;

        const int w = 7, h = 6;

        Counter[,] pieces = new Counter[w, h];

        int place = -1;

        readonly AnimManager animations;

        public FourInARowGame(MainScreen screen, int seed) : base(screen, seed)
        {
            animations = new AnimManager();
            //pieces = new CFourPiece[w, h];
        }

        protected override void RelayMessage2(NetIncomingMessage msg)
        {
            Drop(msg.ReadInt32(), !playerWhite);
        }

        protected override void ResetGame()
        {
            turn ^= true;

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    if (pieces[x, y] != null)
                    {
                        animations.Add(new FallAnim(pieces[x, y].GetTexture,
                                                    new Vector2(x, y) + Vector2.One / 2,
                                                    null,
                                                    null,
                                                    0.05f,
                                                    0.5f,
                                                    0,
                                                    0f,
                                                    15));
                    }

                    pieces[x, y] = null;
                }
                    
        }

        public override void AfterUpdate(GameTime gameTime)
        {
            if (animations.Count > 0)
            {
                animations.Update(gameTime);
                if (animations.Count == 0)
                {
                    SwapTurn();
                }
            } else
            {
                place = -1;

                if (turn == playerWhite)
                {
                    Point mousePos = Vector2.Transform(Input.MousePosition.ToVector2(),
                                                   Matrix.Invert(matrix)).ToPoint();

                    if (place != mousePos.X && mousePos.X >= 0 && mousePos.X < w && ColumnEmptyLength(mousePos.X) != -1)
                    {
                        place = mousePos.X;
                    }
                }
                
                if (place != -1 && Input.LeftMousePressed())
                {
                    /*
                    int b = ColumnEmptyLength(place);

                    var newPiece = new CFourPiece(playerWhite);
                    
                    animations.Add(new FallAnim(newPiece.GetTexture,
                                                new Vector2(place + 0.5f, -1),
                                                () =>
                                                {
                                                    pieces[place, b] = newPiece;
                                                },
                                                0.1f, 1f, 3, 0.4f, b + 0.5f));
                    */

                    Drop(place, playerWhite);

                    SendGameMessage(msg => msg.Write(place));
                }
            }
        }

        int ColumnEmptyLength(int col)
        {
            for (int i = h - 1; i >= 0; i--)
            {
                if (pieces[col, i] == null) return i;
            }

            return -1;
        }

        void Drop(int col, bool white)
        {
            int b = ColumnEmptyLength(col);

            var newPiece = new Counter(white);

            animations.Add(new FallAnim(newPiece.GetTexture,
                                        new Vector2(col + 0.5f, -1),
                                        () =>
                                        {
                                            pieces[col, b] = newPiece;
                                            PostMove(col, b);
                                        },
                                        () =>
                                        {
                                            bounceSound.Play();
                                        },
                                        0.05f,       // Gravity
                                        0.5f,         // Terminal Velocity
                                        3,          // Bounces
                                        0.7f,       // Bounce Multiplier
                                        b + 0.5f    // Floor
                                        ));

        }

        void PostMove(int x, int y)
        {
            if (LineLengthThreshold(x, y, 1, 0, n => n >= 4) ||
                LineLengthThreshold(x, y, 0, 1, n => n >= 4) ||
                LineLengthThreshold(x, y, 1, 1, n => n >= 4) ||
                LineLengthThreshold(x, y, 1, -1, n => n >= 4))
            {
                GameEnd("Four in a Row!" + Environment.NewLine + (turn ? "You win!" : "You lose!"));
            } else
            {
                if (IsADraw())
                    GameEnd("It's a tie!");
            }

            place = -1;
        }

        bool IsADraw()
        {
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (pieces[x, y] == null)
                        return false;
                }
            }

            return true;
        }

        bool InBounds(int x, int y) 
            => x >= 0 && x < w && y >= 0 && y < h;

        bool LineLengthThreshold(int x, int y, int dx, int dy, Func<int, bool> func)
        {
            bool w = pieces[x, y].white;

            int j = x;
            int k = y;

            int num = 1;

            while(true)
            {
                j += dx;
                k += dy;

                if (!InBounds(j, k) || pieces[j, k] == null || pieces[j, k].white != w)
                    break;

                num++;
                if (func(num)) return true;
            }

            j = x;
            k = y;

            while (true)
            {
                j -= dx;
                k -= dy;

                if (!InBounds(j, k) || pieces[j, k] == null || pieces[j, k].white != w)
                    break;

                num++;
                if (func(num)) return true;
            }

            return false;
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: matrix);

            animations.Show(gameTime, spriteBatch);

            // Draw pieces
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Rectangle r = new Rectangle(x, y, 1, 1);

                    pieces[x, y]?.Show(spriteBatch, r);
                }
            }

            // Draw animations
            animations.Show(gameTime, spriteBatch);

            // Draw overlay
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Rectangle r = new Rectangle(x, y, 1, 1);

                    spriteBatch.Draw(box, r, Color.White);
                }
            }

            // Draw arrow
            if (place != -1 && animations.Count == 0)
            {
                Rectangle rect = new Rectangle(place, -1, 1, 1);

                spriteBatch.Draw(playerWhite ? redArrow : yellowArrow, rect, Color.White);
            }

            spriteBatch.End();

            base.Show(gameTime, spriteBatch);
        }
    }
}

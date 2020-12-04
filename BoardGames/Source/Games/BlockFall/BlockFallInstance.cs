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
    public partial class BlockFallGame
    {
        internal class BlockFallInstance : Component
        {
            static readonly Texture2D[] textures;
            static BlockFallInstance()
            {
                textures = new Texture2D[7];
                for (int i = 0; i < 7; i++)
                {
                    textures[i] = Main.main.Content.Load<Texture2D>($"BlockFall/{i}");
                }
            }

            struct Block
            {
                public Block(int[,] _tiles)
                {
                    pos = new Point(4, 0);
                    tiles = _tiles;
                }

                public Point pos;
                public int[,] tiles;

                public int Width => tiles.GetUpperBound(0) + 1;
                public int Height => tiles.GetUpperBound(1) + 1;

                public int LeftMost
                {
                    get
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                if (tiles[x, y] != 0) return x + pos.X;
                            }
                        }

                        throw new InvalidOperationException("There are no tiles in this block.");
                    }
                }
                public int RightMost
                {
                    get
                    {
                        for (int x = Width - 1; x >= 0; x--)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                if (tiles[x, y] != 0) return x + pos.X;
                            }
                        }

                        throw new InvalidOperationException("There are no tiles in this block.");
                    }
                }

                public int BottomMost
                {
                    get
                    {
                        for (int y = Height - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < Width; x++)
                            {
                                if (tiles[x, y] != 0) return y + pos.Y;
                            }
                        }

                        throw new InvalidOperationException("There are no tiles in this block.");
                    }
                }

                public void Show(SpriteBatch spriteBatch)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            if (tiles[x, y] != 0)
                                spriteBatch.Draw(textures[tiles[x, y] - 1],
                                                 new Rectangle(pos.X + x, pos.Y + y, 1, 1),
                                                 Color.White);
                        }
                    }
                }


                public bool TryFit(int[,] landed)
                {
                    // Fit grid
                    int t = LeftMost;
                    if (t < 0) pos.X -= t;

                    t = RightMost;
                    if (t > w - 1) pos.X -= RightMost - w + 1;

                    t = BottomMost;
                    if (t > h - 1) pos.Y -= RightMost - h + 1;

                    return !Overlapping(landed);
                }


                public bool Overlapping(int[,] landed)
                {
                    if (LeftMost < 0 || RightMost > w - 1 || BottomMost > h - 1) return true;

                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            if (tiles[x, y] == 0) continue;



                            //if (x + pos.X < 0) return true;
                            //if (x + pos.X >= w) return true;
                            if (y + pos.Y >= h) return true;

                            if (landed[x + pos.X, y + pos.Y] != 0)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                /*
                public void RotateClockwise()
                {
                    int[,] newTiles = new int[Width, Height];

                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {

                            newTiles[x, y] = tiles[y, 3 - x];
                        }
                    }
                }
                */

                public void RotateClockwise()
                {
                    int j = 0;
                    int p = 0;
                    int q = 0;
                    int i = Width - 1;
                    int[,] rotatedArr = new int[Width, Height];

                    //for (int i = m-1; i >= 0; i--)
                    for (int k = 0; k < Width; k++)
                    {

                        while (i >= 0)
                        {
                            rotatedArr[p, q] = tiles[i, j];
                            q++;
                            i--;
                        }
                        j++;
                        i = Width - 1;
                        q = 0;
                        p++;

                    }

                    tiles = rotatedArr;
                }

                public void RotateAnticlockwise()
                {
                    for (int i = 0; i < 3; i++)
                        RotateClockwise();
                }
            }

            readonly BlockFallGame game;
            readonly Random rnd;

            readonly bool side;

            public BlockFallInstance(BlockFallGame _game, Random _rnd, bool _side)
            {
                game = _game;
                
                landed = new int[w, h];
                rnd = _rnd;
                side = _side;
            }

            public void Reset()
            {
                landed = new int[w, h];
                //hasLanded = true;
                //NewPiece();
            }

            Block current;

            public const int w = 10, h = 16;

            int[,] landed = new int[w, h];

            public void NewPiece()
            {
                
                current = new Block(BlockTypes.PickRandom(rnd));
                if (!current.TryFit(landed))
                    game.GameEnd("You " + ((side != Networking.IsServer) ? "win!" : "lose!"));
            }

            int fallClock;
            const int fallSpeed = 15;
            const int fallFastSpeed = 3;

            public override void Update(GameTime gameTime)
            {
                fallClock++;
                if (fallClock >= ((hasLanded || Input.KeyDown(Keys.Down)) ? fallFastSpeed : fallSpeed))
                {
                    fallClock = 0;

                    Fall();

                    SendGameMessage(msg => msg.Write((byte)TetrisMsgType.DROP));
                }

                if (Input.KeyPressed(Keys.Right))
                {
                    Move(1);

                    SendGameMessage(msg => msg.Write((byte)TetrisMsgType.MOVE_RIGHT));
                }
                if (Input.KeyPressed(Keys.Left))
                {
                    Move(-1);

                    SendGameMessage(msg => msg.Write((byte)TetrisMsgType.MOVE_LEFT));
                }
                if (Input.KeyPressed(Keys.D))
                {
                    RotateClockwise();

                    SendGameMessage(msg => msg.Write((byte)TetrisMsgType.CLOCKWISE_ROT));
                }
                if (Input.KeyPressed(Keys.A))
                {
                    RotateAnticlockwise();

                    SendGameMessage(msg => msg.Write((byte)TetrisMsgType.ANTICLOCKWISE_ROT));
                }
            }

            public void Move(int dX)
            {
                Block newBlock = current;
                newBlock.pos.X += dX;

                if (newBlock.TryFit(landed)) current = newBlock;
            }

            public void RotateClockwise()
            {
                RotateSandwich(b => { b.RotateClockwise(); return b; });
            }

            public void RotateAnticlockwise()
            {
                RotateSandwich(b => { b.RotateAnticlockwise(); return b; });
            }

            // Lol i didn't know what to call this
            void RotateSandwich(Func<Block, Block> modification)
            {
                Block lastBlock = current;
                Block newBlock = current;

                newBlock = modification(newBlock);

                if (newBlock.TryFit(landed)) current = newBlock;
                else
                {
                    Point p = newBlock.pos;

                    for (int i = 0; i < lastBlock.LeftMost - newBlock.LeftMost; i++)
                    {
                        newBlock.pos.X++;
                        if (!newBlock.Overlapping(landed)) { current = newBlock; return; }
                    }

                    newBlock.pos = p;

                    for (int i = 0; i < newBlock.RightMost - lastBlock.RightMost; i++)
                    {
                        newBlock.pos.X--;
                        if (!newBlock.Overlapping(landed)) { current = newBlock; return; }
                    }

                    newBlock.pos = p;

                    for (int i = 0; i < newBlock.BottomMost - lastBlock.BottomMost; i++)
                    {
                        newBlock.pos.Y--;
                        if (!newBlock.Overlapping(landed)) { current = newBlock; return; }
                    }

                    newBlock.pos = p;
                }
            }

            bool hasLanded = true;

            public void Fall()
            {
                Block nextBlock = current;
                nextBlock.pos.Y++;

                if (hasLanded)
                {
                    if (!ClearLine())
                    {
                        NewPiece();
                        hasLanded = false;
                    }
                }
                else
                {
                    if (nextBlock.Overlapping(landed))
                    {
                        Land();
                        hasLanded = true;
                    }
                    else
                    {
                        current = nextBlock;
                    }
                }

            }

            bool ClearLine()
            {
                int y = -1;
                for (int i = h - 1; i >= 0; i--)
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (landed[x, i] == 0) goto nextLoop;
                    }

                    y = i;
                    break;

                nextLoop:
                    ;
                }

                if (y == -1) return false;

                for (int i = y; i > 0; i--)
                {
                    for (int x = 0; x < w; x++)
                    {
                        landed[x, i] = landed[x, i - 1];
                    }
                }

                return true;
            }

            void Land()
            {
                for (int x = 0; x < current.Width; x++)
                {
                    for (int y = 0; y < current.Height; y++)
                    {
                        if (current.tiles[x, y] != 0)
                            landed[x + current.pos.X, y + current.pos.Y] = current.tiles[x, y];
                    }
                }
            }

            static readonly Color backC = new Color(25, 25, 25);

            public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(SpecialContent.Pixel,
                                 new Rectangle(0, 0, w, h),
                                 backC);

                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        if (landed[x, y] != 0)
                            spriteBatch.Draw(textures[landed[x, y] - 1],
                                             new Rectangle(x, y, 1, 1),
                                             Color.White);

                    }
                }

                if (!hasLanded) current.Show(spriteBatch);

            }
        }
    }
    
}

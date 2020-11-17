using BoardGames.Source.Animations;
using BoardGames.Source.FunScreen;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames.DotsAndBoxes
{
    public class DotsAndBoxesGame : TurnGame
    {
        const int w = 7, h = 5, scl = 64;

        enum ColourType
        {
            NONE,
            RED,
            BLUE,
        }

        static Color GetColour(ColourType type)
        {
            switch (type)
            {
                default:
                    return Color.Transparent;
                case ColourType.RED:
                    return Color.Red;
                case ColourType.BLUE:
                    return Color.Blue;
            }
        }


        ColourType[,] horLines, verLines;
        ColourType[,] boxes;


        protected override void ResetGame()
        {
            horLines = new ColourType[w - 1, h];
            verLines = new ColourType[w, h - 1];

            boxes = new ColourType[w - 1, h - 1];
        }

        static Matrix boardMatrix;

        static Texture2D circle, no, scribble;

        protected override string WhiteStr => "red";
        protected override string BlackStr => "blue";

        const float lineThickness = 0.1f;

        readonly AnimManager animations;

        static DotsAndBoxesGame()
        {
            boardMatrix = Matrix.CreateScale(scl) *
                          Matrix.CreateTranslation(new Vector3(wh / 2 - w * scl / 2, wh / 2 - h * scl / 2, 0f));

            circle = Main.main.Content.Load<Texture2D>("Circle");
            no = Main.main.Content.Load<Texture2D>("DotsAndBoxes/No");
            scribble = Main.main.Content.Load<Texture2D>("DotsAndBoxes/Scribble");
        }

        public DotsAndBoxesGame(MainScreen screen, bool white) : base(screen, white)
        {
            animations = new AnimManager();
            //pieces = new CFourPiece[w, h];

            ResetGame();
        }

        Vector2 mousePos;
        Vector2? startPos;

        bool possible;

        public override void AfterUpdate(GameTime gameTime)
        {
            if (turn == playerWhite)
            {
                var p = Input.MousePositionMatrix(boardMatrix);
                MoveMouse(p.X, p.Y);

                var msg = CreateGameMessage();
                msg.Write((byte)IncomingType.MOVE);
                msg.Write(mousePos.X);
                msg.Write(mousePos.Y);
                Networking.SendMessage(msg, NetDeliveryMethod.ReliableSequenced, 1);

                
                if (Input.LeftMousePressed() && mousePos.X >= 0 && mousePos.X < w && mousePos.Y >= 0 && mousePos.Y < h)
                {
                    StartLine();

                    var msg2 = CreateGameMessage();
                    msg2.Write((byte)IncomingType.START);
                    Networking.SendMessage(msg2);
                }
                else if (Input.LeftMouseUp())
                {
                    ReleaseLine();

                    var msg2 = CreateGameMessage();
                    msg2.Write((byte)IncomingType.RELEASE);
                    Networking.SendMessage(msg2);
                }

                
            }
        }

        void StartLine()
        {
            startPos = new Vector2((int)mousePos.X, (int)mousePos.Y);
        }

        void ReleaseLine()
        {
            if (startPos != null)
            {
                Point s = startPos.Value.ToPoint();
                Point m = mousePos.ToPoint();

                if (possible && s != m)
                {
                    CreateLine(turn ? ColourType.RED : ColourType.BLUE, s.X, m.X, s.Y, m.Y);
                }
            }

            startPos = null;
        }

        void MoveMouse(float x, float y)
        {
            //mousePos = Input.MousePositionMatrix(boardMatrix);

            mousePos = new Vector2(x, y);

            if (startPos != null)
            {
                Point p = mousePos.ToPoint();

                possible = (startPos.Value.ToPoint() == p) ||
                           ((p.X == startPos.Value.X || p.Y == startPos.Value.Y) &&
                           (Math.Abs(p.X - startPos.Value.X) + Math.Abs(p.Y - startPos.Value.Y)) == 1) &&
                           GetLine((int)startPos.Value.X, p.X, (int)startPos.Value.Y, p.Y) == ColourType.NONE;
            }
        }

        void CreateLine(ColourType type, int x1, int x2, int y1, int y2)
        {
            int x = Math.Min(x1, x2);
            int y = Math.Min(y1, y2);

            bool filled = false;

            if (y1 == y2)
            {
                horLines[x, y] = type;

                if (y != 0) CheckFillBox(type, x, y - 1, ref filled);
                if (y <= boxes.GetUpperBound(1)) CheckFillBox(type, x, y, ref filled);
            } else
            {
                verLines[x, y] = type;

                if (x != 0) CheckFillBox(type, x - 1, y, ref filled);
                if (x <= boxes.GetUpperBound(0)) CheckFillBox(type, x, y, ref filled);
            }

            if (!filled)
            {
                SwapTurn();
            }

            if (AllBoxesFull(out int red, out int blue))
            {
                if (red == blue)
                {
                    GameEnd($"It's a tie! (Red: {red} Blue: {blue})");
                } else
                {
                    if ((playerWhite && red > blue) || (!playerWhite && blue > red))
                    {
                        GameEnd($"You win! (Red: {red} Blue: {blue})");
                    } else
                    {
                        GameEnd($"You lose! (Red: {red} Blue: {blue})");
                    }
                }
            }
        }

        bool AllBoxesFull(out int red, out int blue)
        {
            red = 0;
            blue = 0;

            for (int j = 0; j <= boxes.GetUpperBound(0); j++)
            {
                for (int k = 0; k <= boxes.GetUpperBound(1); k++)
                {
                    switch(boxes[j, k])
                    {
                        case ColourType.NONE:
                            return false;
                        case ColourType.RED:
                            red++;
                            break;
                        case ColourType.BLUE:
                            blue++;
                            break;
                    }
                }
            }

            return true;
        }

        ColourType GetLine(int x1, int x2, int y1, int y2)
        {
            int x = Math.Min(x1, x2);
            int y = Math.Min(y1, y2);

            if (y1 == y2)
            {
                if (x >= 0 && x <= horLines.GetUpperBound(0) &&
                    y >= 0 && y <= horLines.GetUpperBound(1))
                {
                    return horLines[x, y];
                }
                else return ColourType.RED; // Out of bounds
                
            } else
            {
                if (x >= 0 && x <= verLines.GetUpperBound(0) &&
                    y >= 0 && y <= verLines.GetUpperBound(1))
                {
                    return verLines[x, y];
                }
                else return ColourType.RED; // Out of bounds
            }
        }

        void CheckFillBox(ColourType type, int x, int y, ref bool filled)
        {
            if ((horLines[x, y] != ColourType.NONE) &&
               (horLines[x, y + 1] != ColourType.NONE) &&
               (verLines[x, y] != ColourType.NONE) &&
               (verLines[x + 1, y] != ColourType.NONE))
            {
                boxes[x, y] = type;
                filled = true;
            }
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix:boardMatrix);

            // Draw boxes

            for (int j = 0; j <= boxes.GetUpperBound(0); j++)
            {
                for (int k = 0; k <= boxes.GetUpperBound(1); k++)
                {
                    spriteBatch.Draw(scribble,
                                     new Vector2(j, k) + Vector2.One / 2,
                                     null,
                                     GetColour(boxes[j, k]),
                                     0f,
                                     Vector2.Zero,
                                     Vector2.One / new Vector2(scribble.Width, scribble.Height),
                                     SpriteEffects.None,
                                     0f);
                }
            }

            // Draw lines

            for (int j = 0; j <= horLines.GetUpperBound(0); j++)
            {
                for (int k = 0; k <= horLines.GetUpperBound(1); k++)
                {
                    
                    if (horLines[j, k] != ColourType.NONE)
                    {
                        Color c = GetColour(horLines[j, k]);

                        spriteBatch.Draw(SpecialContent.Pixel,
                                     new Vector2(j, k) + Vector2.One / 2 - new Vector2(lineThickness) / 2,
                                     null,
                                     c,
                                     0f,
                                     Vector2.Zero,
                                     new Vector2(1, lineThickness),
                                     SpriteEffects.None,
                                     0f);
                    }

                }
            }

            for (int j = 0; j <= verLines.GetUpperBound(0); j++)
            {
                for (int k = 0; k <= verLines.GetUpperBound(1); k++)
                {

                    if (verLines[j, k] != ColourType.NONE)
                    {
                        Color c = GetColour(verLines[j, k]);

                        spriteBatch.Draw(SpecialContent.Pixel,
                                     new Vector2(j, k) + Vector2.One / 2 - new Vector2(lineThickness) / 2,
                                     null,
                                     c,
                                     0f,
                                     Vector2.Zero,
                                     new Vector2(lineThickness, 1),
                                     SpriteEffects.None,
                                     0f);
                    }

                }
            }

            // Draw points

            Point p = mousePos.ToPoint();

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    
                    Color c;

                    if ((p.X == x && p.Y == y) || (startPos != null && startPos.Value.X == x && startPos.Value.Y == y))
                        c = Color.Yellow;
                    else
                        c = Color.Beige;

                    spriteBatch.Draw(circle,
                                     new Vector2(x, y) + Vector2.One / 2,
                                     null,
                                     c,
                                     0f,
                                     new Vector2(circle.Width / 2, circle.Height / 2),
                                     Vector2.One / 64,
                                     SpriteEffects.None,
                                     0f);
                }
            }

            // Draw mouse line

            if (startPos != null)
            {
                spriteBatch.DrawLine(startPos.Value + Vector2.One / 2, mousePos, Color.White, lineThickness);
                
                if (!possible)
                {
                    spriteBatch.Draw(no,
                                     (mousePos + startPos.Value + Vector2.One / 2) / 2,
                                     null,
                                     Color.White,
                                     0f,
                                     new Vector2(no.Width, no.Height) / 2,
                                     Vector2.One / 64,
                                     SpriteEffects.None,
                                     0f);
                }
            }

            spriteBatch.End();
            
            base.Show(gameTime, spriteBatch);
        }

        enum IncomingType
        {
            START,
            RELEASE,
            MOVE
        }

        protected override void RelayMessage2(NetIncomingMessage msg)
        {
            switch ((IncomingType)msg.ReadByte())
            {
                case IncomingType.START:
                    StartLine();

                    break;
                case IncomingType.RELEASE:
                    ReleaseLine();

                    break;
                case IncomingType.MOVE:
                    MoveMouse(msg.ReadFloat(), msg.ReadFloat());

                    break;
            }
        }
    }
}

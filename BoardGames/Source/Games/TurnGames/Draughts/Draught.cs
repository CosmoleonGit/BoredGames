using BoardGames.Source.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BoardGames.Source.Games.TurnGames.Draughts
{
    public partial class DraughtsGame
    {
        internal class Draught : MobilePiece
        {
            public Draught(bool white) : base(white) { }

            private bool king;

            public override bool[,] PossibleMoves2(PieceBoard board, int x, int y)
            {
                Point capturing = (board as DraughtsGame).capturingPiece;
                if (capturing.X != -1 && (capturing.X != x || capturing.Y != y)) return new bool[8, 8];

                bool[,] possible = PossibleMoves3(board, x, y, out bool canCapture);

                if (canCapture) return possible;

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (x == i && y == j) continue;

                        if (board.pieces[i, j] != null && board.pieces[i, j].white == white)
                        {
                            (board.pieces[i, j] as Draught).PossibleCaptures(board, i, j, out bool any);

                            if (any)
                            {
                                return new bool[8, 8];
                            }
                        }
                    }
                }
                
                return possible;
            }

            protected bool[,] PossibleMoves3(PieceBoard board, int x, int y, out bool canCapture)
            {
                bool[,] possible = PossibleCaptures(board, x, y, out canCapture);

                if (white || king)
                {
                    if (!canCapture)
                    {
                        if (EmptySpace(board, x - 1, y - 1)) possible[x - 1, y - 1] = true;
                        if (EmptySpace(board, x + 1, y - 1)) possible[x + 1, y - 1] = true;
                    }
                }

                if (!white || king)
                {
                    if (!canCapture)
                    {
                        if (EmptySpace(board, x - 1, y + 1)) possible[x - 1, y + 1] = true;
                        if (EmptySpace(board, x + 1, y + 1)) possible[x + 1, y + 1] = true;
                    }
                }

                return possible;
            }

            internal bool[,] PossibleCaptures(PieceBoard board, int x, int y, out bool any)
            {
                bool[,] possible = new bool[8, 8];

                bool captureMove(int x1, int y1, int x2, int y2)
                {
                    if (EnemySpace(board, x1, y1) && EmptySpace(board, x2, y2))
                    {
                        possible[x2, y2] = true;
                        return true;
                    }
                    return false;
                }

                //canCapture = false;
                any = false;

                if (white || king)
                {
                    if (captureMove(x - 1, y - 1, x - 2, y - 2) | captureMove(x + 1, y - 1, x + 2, y - 2))
                    {
                        any = true;
                    }
                }

                if (!white || king)
                {
                    if (captureMove(x - 1, y + 1, x - 2, y + 2) | captureMove(x + 1, y + 1, x + 2, y + 2))
                    {
                        any = true;
                    }
                }

                return possible;
            }

            protected override void AfterMove(PieceBoard board, int x1, int y1, int x2, int y2)
            {
                if (Math.Abs(x2 - x1) == 2)
                {
                    board.pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                    (board as DraughtsGame).capturingPiece = new Point(x2, y2);
                }

                if (!king && (y2 == 0 || y2 == 7))
                {
                    king = true;
                    MobilePiece piece = this;

                    board.pieces[x2, y2] = null;

                    board.animations.Add(new SpriteAnim(white ? whitePieces.flip : blackPieces.flip,
                                                        new Vector2(x2, y2) + Vector2.One / 2,
                                                        () => board.pieces[x2, y2] = piece));
                }
            }

            protected override Texture2D GetWhiteTex()
            {
                return king ? whitePieces.king : whitePieces.piece;
            }

            protected override Texture2D GetBlackTex()
            {
                return king ? blackPieces.king : blackPieces.piece;
            }
        }
    }
}

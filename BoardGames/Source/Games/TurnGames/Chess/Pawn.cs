using BoardGames.Source.FunScreen;
using BoardGames.Source.Games.SplashScreens;
using Microsoft.Xna.Framework.Graphics;
using MonoNet;
using System;

namespace BoardGames.Source.Games.TurnGames.Chess
{
    public partial class ChessGame
    {
        internal class Pawn : ChessPiece
        {
            public Pawn(bool _white) : base(_white) { }

            protected override Texture2D GetBlackTex()
            {
                return blackPieces.pawn;
            }

            protected override Texture2D GetWhiteTex()
            {
                return whitePieces.pawn;
            }

            public override bool[,] PossibleMoves3(PieceBoard board, int x, int y)
            {
                bool[,] possible = new bool[8, 8];

                if (white)
                {
                    if (EmptySpace(board, x, y - 1))
                    {
                        possible[x, y - 1] = true;

                        if (y == 6 && EmptySpace(board, x, y - 2))
                        {
                            possible[x, y - 2] = true;
                        }
                    }

                    if (EnemySpace(board, x + 1, y - 1)) possible[x + 1, y - 1] = true;
                    if (EnemySpace(board, x - 1, y - 1)) possible[x - 1, y - 1] = true;
                } else
                {
                    if (EmptySpace(board, x, y + 1))
                    {
                        possible[x, y + 1] = true;

                        if (y == 1 && EmptySpace(board, x, y + 2))
                        {
                            possible[x, y + 2] = true;
                        }
                    }

                    if (EnemySpace(board, x + 1, y + 1)) possible[x + 1, y + 1] = true;
                    if (EnemySpace(board, x - 1, y + 1)) possible[x - 1, y + 1] = true;
                }

                return possible;
            }

            public byte turnInto = 0;

            protected override bool CancelMove(PieceBoard board, int x1, int y1, int x2, int y2, Action action = null)
            {
                if (turnInto == 0 && (y2 == 0 || y2 == 7))
                {
                    OptionsOverlay splash = new OptionsOverlay("What will your pawn turn into?", new string[] { "Knight", "Bishop", "Rook", "Queen" })
                    {
                        action = (string s) =>
                        {
                            switch (s)
                            {
                                case "Knight":
                                    turnInto = 1;
                                    break;
                                case "Bishop":
                                    turnInto = 2;
                                    break;
                                case "Rook":
                                    turnInto = 3;
                                    break;
                                case "Queen":
                                    turnInto = 4;
                                    break;
                            }

                            Move(board, x1, y1, x2, y2, action);
                        }
                    };
                    board.overlay = splash;
                    return true;
                } else
                {
                    return false;
                }
            }

            protected override void AfterMove(PieceBoard board, int x1, int y1, int x2, int y2)
            {
                if (y2 == 0 || y2 == 7)
                {
                    switch (turnInto)
                    {
                        case 1:
                            board.pieces[x2, y2] = new Knight(white);
                            break;
                        case 2:
                            board.pieces[x2, y2] = new Bishop(white);
                            break;
                        case 3:
                            board.pieces[x2, y2] = new Rook(white);
                            break;
                        case 4:
                            board.pieces[x2, y2] = new Queen(white);
                            break;
                    }
                }
            }

            public override void BeforeMove(PieceBoard board, int x1, int y1, int x2, int y2)
            {
                if (turnInto == 0)
                {
                    base.BeforeMove(board, x1, y1, x2, y2);
                } else
                {
                    SendGameMessage(msg =>
                    {
                        msg.Write(x1);
                        msg.Write(y1);
                        msg.Write(x2);
                        msg.Write(y2);
                        msg.Write(turnInto);
                    });
                }
                
            }
        }
    }
}

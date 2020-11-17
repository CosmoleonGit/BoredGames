using Microsoft.Xna.Framework;
using System;

namespace BoardGames.Source.Games.TurnGames.Chess
{
    public partial class ChessGame
    {
        internal abstract class ChessPiece : MobilePiece
        {
            protected ChessPiece(bool white) : base(white) { }

            public override bool[,] PossibleMoves2(PieceBoard board, int x, int y)
            {
                bool[,] possible = PossibleMoves3(board, x, y);

                ChessGame chess = board as ChessGame;
                

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (possible[i, j])
                        {
                            MobilePiece overwritePiece = board.pieces[i, j];
                            board.pieces[i, j] = board.pieces[x, y];
                            board.pieces[x, y] = null;

                            Point kingPos = chess.GetKingPos(white);
                            bool check = chess.InCheck(kingPos.X, kingPos.Y, white);
                            //Console.WriteLine(check);

                            board.pieces[x, y] = board.pieces[i, j];
                            board.pieces[i, j] = overwritePiece;

                            if (check) possible[i, j] = false;
                        }
                    }
                }

                return possible;
            }

            public abstract bool[,] PossibleMoves3(PieceBoard board, int x, int y);

            protected void DirectionalPossible(ref bool[,] possible, PieceBoard board, int x, int y, int dX, int dY)
            {
                int sX = x;
                int sY = y;

                while (true)
                {
                    if (!PossibleSpace(board, sX + dX, sY + dY)) break;

                    sX += dX;
                    sY += dY;

                    possible[sX, sY] = true;

                    if (EnemySpace(board, sX, sY)) break;
                }
            }
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames.Chess
{
    public partial class ChessGame
    {
        internal class King : ChessPiece
        {
            public King(bool _white) : base(_white) { }

            protected override Texture2D GetBlackTex()
            {
                return blackPieces.king;
            }

            protected override Texture2D GetWhiteTex()
            {
                return whitePieces.king;
            }

            public override bool[,] PossibleMoves3(PieceBoard board, int x, int y)
            {
                bool[,] possible = new bool[8, 8];

                if (PossibleSpace(board, x + 1, y)) possible[x + 1, y] = true;
                if (PossibleSpace(board, x + 1, y + 1)) possible[x + 1, y + 1] = true;
                if (PossibleSpace(board, x + 1, y - 1)) possible[x + 1, y - 1] = true;
                if (PossibleSpace(board, x - 1, y)) possible[x - 1, y] = true;
                if (PossibleSpace(board, x - 1, y + 1)) possible[x - 1, y + 1] = true;
                if (PossibleSpace(board, x - 1, y - 1)) possible[x - 1, y - 1] = true;
                if (PossibleSpace(board, x, y + 1)) possible[x, y + 1] = true;
                if (PossibleSpace(board, x, y - 1)) possible[x, y - 1] = true;

                if (!moved)
                {
                    ChessGame chess = board as ChessGame;

                    if (board.pieces[5, y] == null && board.pieces[6, y] == null &&
                        board.pieces[7, y] is Rook r1 && !r1.moved && r1.white == white &&
                        !chess.InCheck(5, y, white) && !chess.InCheck(6, y, white))
                    {
                        possible[6, y] = true;
                    }

                    if (board.pieces[3, y] == null && board.pieces[2, y] == null && board.pieces[1, y] == null &&
                        board.pieces[0, y] is Rook r2 && !r2.moved && r2.white == white &&
                        !chess.InCheck(3, y, white) && !chess.InCheck(2, y, white))
                    {
                        possible[2, y] = true;
                    }
                }

                return possible;
            }
            
            protected override bool CancelMove(PieceBoard board, int x1, int y1, int x2, int y2, Action action = null)
            {
                if (x2 == x1 + 2)
                {
                    (board.pieces[7, y1] as Rook).Move(board, 7, y1, 5, y1);
                } else if (x2 == x1 - 2)
                {
                    (board.pieces[0, y1] as Rook).Move(board, 0, y1, 3, y1);
                }

                return false;
            }
        }
    }
}

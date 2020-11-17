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
        internal class Knight : ChessPiece
        {
            public Knight(bool _white) : base(_white) { }

            protected override Texture2D GetBlackTex()
            {
                return blackPieces.knight;
            }

            protected override Texture2D GetWhiteTex()
            {
                return whitePieces.knight;
            }

            public override bool[,] PossibleMoves3(PieceBoard board, int x, int y)
            {
                bool[,] possible = new bool[8, 8];

                if (PossibleSpace(board, x + 2, y + 1)) possible[x + 2, y + 1] = true;
                if (PossibleSpace(board, x + 2, y - 1)) possible[x + 2, y - 1] = true;
                if (PossibleSpace(board, x + 1, y - 2)) possible[x + 1, y - 2] = true;
                if (PossibleSpace(board, x - 1, y - 2)) possible[x - 1, y - 2] = true;
                if (PossibleSpace(board, x - 2, y + 1)) possible[x - 2, y + 1] = true;
                if (PossibleSpace(board, x - 2, y - 1)) possible[x - 2, y - 1] = true;
                if (PossibleSpace(board, x + 1, y + 2)) possible[x + 1, y + 2] = true;
                if (PossibleSpace(board, x - 1, y + 2)) possible[x - 1, y + 2] = true;

                return possible;
            }
        }
    }
}

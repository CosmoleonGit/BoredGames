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
        internal class Rook : ChessPiece
        {
            public Rook(bool _white) : base(_white) { }

            protected override Texture2D GetBlackTex()
            {
                return blackPieces.rook;
            }

            protected override Texture2D GetWhiteTex()
            {
                return whitePieces.rook;
            }

            public override bool[,] PossibleMoves3(PieceBoard board, int x, int y)
            {
                bool[,] possible = new bool[8, 8];

                DirectionalPossible(ref possible, board, x, y, 1, 0);
                DirectionalPossible(ref possible, board, x, y, -1, 0);
                DirectionalPossible(ref possible, board, x, y, 0, 1);
                DirectionalPossible(ref possible, board, x, y, 0, -1);

                return possible;
            }
        }
    }
}

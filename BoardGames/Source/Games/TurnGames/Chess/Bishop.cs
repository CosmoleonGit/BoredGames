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
        internal class Bishop : ChessPiece
        {
            public Bishop(bool _white) : base(_white) { }

            protected override Texture2D GetBlackTex()
            {
                return blackPieces.bishop;
            }

            protected override Texture2D GetWhiteTex()
            {
                return whitePieces.bishop;
            }

            public override bool[,] PossibleMoves3(PieceBoard board, int x, int y)
            {
                bool[,] possible = new bool[8, 8];

                DirectionalPossible(ref possible, board, x, y, 1, 1);
                DirectionalPossible(ref possible, board, x, y, -1, 1);
                DirectionalPossible(ref possible, board, x, y, 1, -1);
                DirectionalPossible(ref possible, board, x, y, -1, -1);

                return possible;
            }
        }
    }
}

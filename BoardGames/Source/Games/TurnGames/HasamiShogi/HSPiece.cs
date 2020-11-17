using BoardGames.Source.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BoardGames.Source.Games.TurnGames.HasamiShogi
{
    public partial class HasamiShogiGame
    {
        internal class HSPiece : MobilePiece
        {
            public HSPiece(bool white) : base(white) { }

            public override bool[,] PossibleMoves2(PieceBoard board, int x, int y)
            {
                bool[,] possible = new bool[9, 9];

                DirectionalPossible(ref possible, board, x, y, 1, 0);
                DirectionalPossible(ref possible, board, x, y, -1, 0);
                DirectionalPossible(ref possible, board, x, y, 0, 1);
                DirectionalPossible(ref possible, board, x, y, 0, -1);

                return possible;
            }

            protected override Texture2D GetBlackTex()
            {
                return blue;
            }

            protected override Texture2D GetWhiteTex()
            {
                return red;
            }

            void DirectionalPossible(ref bool[,] possible, PieceBoard board, int x, int y, int dX, int dY)
            {
                int sX = x;
                int sY = y;

                while (true)
                {
                    if (!EmptySpace(board, sX + dX, sY + dY)) break;

                    sX += dX;
                    sY += dY;

                    possible[sX, sY] = true;
                }
            }

            void LineCapture(PieceBoard board, int x, int y, int dX, int dY)
            {
                int sX = x;
                int sY = y;

                int i = 0;
                while (true)
                {
                    sX += dX;
                    sY += dY;


                    if (!board.InBounds(sX, sY) || EmptySpace(board, sX, sY)) return;
                    else if (FriendSpace(board, sX, sY)) break;

                    i++;
                }

                sX = x; sY = y;

                for (int j = 0; j < i; j++)
                {
                    sX += dX; 
                    sY += dY;

                    CapturePiece(board, sX, sY);
                }
            }

            void CapturePiece(PieceBoard board, int x, int y)
            {
                board.animations.Add(new ZoomAnim(board.pieces[x, y].GetTexture, new Vector2(x, y), null, Vector2.One, Vector2.Zero));
                board.pieces[x, y] = null;
            }

            protected override void AfterMove(PieceBoard board, int x1, int y1, int x2, int y2)
            {
                LineCapture(board, x2, y2, 1, 0);
                LineCapture(board, x2, y2, -1, 0);
                LineCapture(board, x2, y2, 0, 1);
                LineCapture(board, x2, y2, 0, -1);

                // Corner captures
                void CheckCorner(int j, int k)
                {
                    int nJ = (j == 0) ? 1 : 7;
                    int nK = (k == 0) ? 1 : 7;

                    if (board.ComparePieceColours(board.pieces[nJ, k], board.pieces[j, nK]) &&
                        board.pieces[j, k] != null && board.pieces[j, k].white != board.pieces[nJ, k].white)
                        CapturePiece(board, j, k);
                }

                CheckCorner(0, 0);
                CheckCorner(8, 0);
                CheckCorner(0, 8);
                CheckCorner(8, 8);
            }
        }
    }
}

using BoardGames.Source.Animations;
using BoardGames.Source.FunScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet;
using System;
using System.Collections.Generic;

namespace BoardGames.Source.Games.TurnGames
{
    public partial class PieceBoard
    {
        public abstract class MobilePiece : Piece
        {
            public MobilePiece(bool white) : base(white) { }

            public bool[,] PossibleMoves(PieceBoard board, int x, int y)
            {
                //if (board.turn != board.playerWhite || white != board.playerWhite)
                if (white != board.turn || board.turn != board.playerWhite)
                {
                    return new bool[9, 9];
                } else
                {
                    return PossibleMoves2(board, x, y);
                }
            }

            public abstract bool[,] PossibleMoves2(PieceBoard board, int x, int y);

            public bool moved;

            public void Move(PieceBoard board, int x1, int y1, int x2, int y2, Action action = null)
            {
                if (!CancelMove(board, x1, y1, x2, y2, action))
                {
                    //board.pieces[x1, y1].BeforeMove(board, x1, y1, x2, y2);
                    action?.Invoke();

                    MobilePiece piece = board.pieces[x1, y1];
                    piece.moved = true;
                    board.pieces[x1, y1] = null;

                    Vector2 dest = new Vector2(x2, y2) + Vector2.One / 2;

                    board.animations.Add(new MoveAnim(piece.GetTexture,
                                         new Vector2(x1, y1) + Vector2.One / 2,
                                         dest,
                                         () => 
                                         { 
                                             moveSound.Play(); 
                                             Point destPos = dest.ToPoint();
                                             board.pieces[destPos.X, destPos.Y] = piece;
                                             AfterMove(board, x1, y1, x2, y2);
                                         }));
                }
            }

            public virtual void BeforeMove(PieceBoard board, int x1, int y1, int x2, int y2)
            {
                //board.SendMessage(x1 + "," + y1 + "," + x2 + "," + y2);

                var msg = CreateGameMessage();

                msg.Write(x1);
                msg.Write(y1);
                msg.Write(x2);
                msg.Write(y2);

                Networking.SendMessage(msg);
            }

            protected virtual bool CancelMove(PieceBoard board, int x1, int y1, int x2, int y2, Action action = null) { return false; }
            protected virtual void AfterMove(PieceBoard board, int x1, int y1, int x2, int y2) { }

            protected static bool EmptySpace(PieceBoard board, int x, int y)
            {
                return board.InBounds(x, y) && board.pieces[x, y] == null;
            }

            protected bool FriendSpace(PieceBoard board, int x, int y)
            {
                return board.InBounds(x, y) && board.pieces[x, y] != null && board.pieces[x, y].white == white;
            }

            protected bool EnemySpace(PieceBoard board, int x, int y)
            {
                return board.InBounds(x, y) && board.pieces[x, y] != null && board.pieces[x, y].white != white;
            }

            protected bool PossibleSpace(PieceBoard board, int x, int y)
            {
                return EmptySpace(board, x, y) || EnemySpace(board, x, y);
            }
        }
    }
}

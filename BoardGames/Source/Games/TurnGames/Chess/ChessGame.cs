using BoardGames.Source.FunScreen;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames.Chess
{
    public partial class ChessGame : PieceBoard
    {
        public ChessGame(MainScreen main, bool white) : base(main, white, 8) { }

        static SoundEffect warning;

        /*
        static ChessGame()
        {
            GameContent.OnLoadContent += delegate (ContentManager content)
            {
                whitePieces = new ColourPieces(content, "white");
                blackPieces = new ColourPieces(content, "black");
                warning = content.Load<SoundEffect>("Chess/LightWarning");
            };
        }
        */

        static ChessGame()
        {
            whitePieces = new ColourPieces(Main.main.Content, "white");
            blackPieces = new ColourPieces(Main.main.Content, "black");
            warning = Main.main.Content.Load<SoundEffect>("Chess/LightWarning");
        }
        
        /*
        public static void LoadPieces(ContentManager content)
        {
            whitePieces = new ColourPieces(content, "white");
            blackPieces = new ColourPieces(content, "black");
            warning = content.Load<SoundEffect>("Chess/LightWarning");
        }
        */

        private struct ColourPieces
        {
            public ColourPieces(ContentManager content, string colourName)
            {
                pawn = content.Load<Texture2D>("Chess/" + colourName + "_pawn");
                knight = content.Load<Texture2D>("Chess/" + colourName + "_knight");
                bishop = content.Load<Texture2D>("Chess/" + colourName + "_bishop");
                rook = content.Load<Texture2D>("Chess/" + colourName + "_rook");
                queen = content.Load<Texture2D>("Chess/" + colourName + "_queen");
                king = content.Load<Texture2D>("Chess/" + colourName + "_king");
            }

            public Texture2D pawn,
                             knight,
                             bishop,
                             rook,
                             queen,
                             king;
        }

        static ColourPieces whitePieces, blackPieces;

        protected override void ResetGame()
        {
            playerWhite ^= true;
            turn = true;

            base.ResetGame();

            // Load pawns
            for (int x = 0; x < 8; x++)
            {
                SetPiece(new Pawn(false), x, 1);
                SetPiece(new Pawn(true), x, 6);
            }

            // Load knights
            SetPiece(new Knight(false), 1, 0);
            SetPiece(new Knight(false), 6, 0);

            SetPiece(new Knight(true), 1, 7);
            SetPiece(new Knight(true), 6, 7);

            // Load bishops
            SetPiece(new Bishop(false), 2, 0);
            SetPiece(new Bishop(false), 5, 0);

            SetPiece(new Bishop(true), 2, 7);
            SetPiece(new Bishop(true), 5, 7);

            // Load rooks
            SetPiece(new Rook(false), 0, 0);
            SetPiece(new Rook(false), 7, 0);

            SetPiece(new Rook(true), 0, 7);
            SetPiece(new Rook(true), 7, 7);

            // Load queens
            SetPiece(new Queen(false), 3, 0);
            SetPiece(new Queen(true), 3, 7);
            
            // Load kings
            SetPiece(new King(false), 4, 0);
            SetPiece(new King(true), 4, 7);
        }

        protected override void AfterMove()
        {
            SwapTurn();

            base.AfterMove();


            if (!CanMove(turn))
            {
                Point king = GetKingPos(turn);

                if (InCheck(king.X, king.Y, turn))
                {
                    if (turn == playerWhite)
                    {
                        GameEnd("Checkmate! You lost!");
                    }
                    else
                    {
                        GameEnd("Checkmate! You win!");
                    }
                } else
                {
                    GameEnd("Stalemate!");
                }
                
            } else if (turn == playerWhite)
            {
                Point king = GetKingPos(playerWhite);

                if (InCheck(king.X, king.Y, playerWhite))
                {
                    warning.Play();
                }
            }

        }

        protected Point GetKingPos(bool white)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (pieces[x, y] != null && pieces[x, y].white == white && pieces[x, y] is King)
                    {
                        return new Point(x, y);
                    }
                }
            }

            throw new InvalidOperationException("No king of this colour exists.");
        }

        public bool InCheck(int x, int y, bool white)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (x == i && y == j) continue;

                    MobilePiece piece = pieces[i, j];
                    if (piece != null && piece.white != white)
                    {
                        if ((piece as ChessPiece).PossibleMoves3(this, i, j)[x, y]) return true;
                    }
                }
            }

            return false;
        }

        protected override void RelayMessage2(NetIncomingMessage msg)
        {
            int x1 = msg.ReadInt32(),
                y1 = msg.ReadInt32(),
                x2 = msg.ReadInt32(),
                y2 = msg.ReadInt32();

            if (pieces[x1, y1] != null && y2 == (pieces[x1, y1].white ? 0 : 7) && pieces[x1, y1] is Pawn pawn)
            {
                pawn.turnInto = msg.ReadByte();
            }

            pieces[x1, y1]?.Move(this, x1, y1, x2, y2);
        }
    }
}

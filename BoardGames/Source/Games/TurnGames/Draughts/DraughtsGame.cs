using BoardGames.Source.FunScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

namespace BoardGames.Source.Games.TurnGames.Draughts
{
    public partial class DraughtsGame : PieceBoard
    {
        public DraughtsGame(MainScreen main, int seed) : base(main, seed, 8) { }

        static DraughtsGame()
        {
            whitePieces = new ColourPieces(Main.main.Content, "White");
            blackPieces = new ColourPieces(Main.main.Content, "Black");
        }
        
        /*
        public static void LoadPieces(ContentManager content)
        {
            whitePieces = new ColourPieces(content, "White");
            blackPieces = new ColourPieces(content, "Black");
        }
        */
        
        
        protected struct ColourPieces
        {
            public ColourPieces(ContentManager content, string colourName)
            {
                piece = content.Load<Texture2D>("Draughts/" + colourName + "_piece");
                king = content.Load<Texture2D>("Draughts/crowned_" + colourName + "_piece");
                flip = new Texture2D[5];
                for (int i = 0; i < flipFrames; i++)
                {
                    flip[i] = content.Load<Texture2D>("Draughts/" + colourName + "_turn_" + i.ToString());
                }
            }

            public readonly Texture2D piece, king;
            public readonly Texture2D[] flip;
        }

        static ColourPieces whitePieces, blackPieces;

        const int flipFrames = 5;

        Point capturingPiece;

        protected override bool ColourFirst => false;

        protected override void ResetGame()
        {
            playerWhite ^= true;
            turn = false;

            base.ResetGame();

            capturingPiece = new Point(-1);

            pieces = new MobilePiece[8, 8];

            LoopThroughAll((x, y) =>
            {
                if (y < 3)
                {
                    if ((x + y) % 2 == 1)
                    {
                        SetPiece(new Draught(false), x, y);
                    }
                }
                else if (y > 4)
                {
                    if ((x + y) % 2 == 1)
                    {
                        SetPiece(new Draught(true), x, y);
                    }
                }
            });
        }

        protected override void AfterMove()
        {
            

            if (capturingPiece.X != -1)
            {
                Draught capturePiece = pieces[capturingPiece.X, capturingPiece.Y] as Draught;

                if (capturePiece.white)
                {
                    if (capturingPiece.Y == 0) goto changeTurn;
                } else
                {
                    if (capturingPiece.Y == 7) goto changeTurn;
                }

                capturePiece.PossibleCaptures(this, capturingPiece.X, capturingPiece.Y, out bool any);
                if (!any) goto changeTurn;
            } else
            {
                goto changeTurn;
            }

            return;

        changeTurn:

            SwapTurn();
            capturingPiece = new Point(-1);

            base.AfterMove();

            if (!ColourCountThreshold(turn, x => x > 0))
            {
                if (turn == playerWhite)
                {
                    GameEnd("You lost!");
                }
                else
                {
                    GameEnd("You win!");
                }
            }
            else if (!CanMove(turn))
            {
                GameEnd("It's a draw!");
            }
        }
    }
}

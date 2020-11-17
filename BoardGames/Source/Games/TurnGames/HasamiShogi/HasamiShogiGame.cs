using BoardGames.Source.FunScreen;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames.HasamiShogi
{
    public partial class HasamiShogiGame : PieceBoard
    {
        static Texture2D red, blue;

        static HasamiShogiGame()
        {
            red = Main.main.Content.Load<Texture2D>("HasamiShogi/red");
            blue = Main.main.Content.Load<Texture2D>("HasamiShogi/blue");
        }

        protected override string WhiteStr => "red";
        protected override string BlackStr => "blue";

        public HasamiShogiGame(MainScreen main, bool white) : base(main, white, 9) { }

        protected override void ResetGame()
        {
            base.ResetGame();

            for (int x = 0; x < 9; x++)
            {
                SetPiece(new HSPiece(false), x, 0);
                SetPiece(new HSPiece(true), x, 8);
            }
        }

        protected override void AfterMove()
        {
            SwapTurn();

            if (!ColourCountThreshold(turn, x => x > 2))
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

            base.AfterMove();
        }
    }
}

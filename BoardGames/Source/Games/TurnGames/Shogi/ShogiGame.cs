using BoardGames.Source.FunScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TurnGames.Shogi
{
    public class ShogiGame : PieceBoard
    {
        public ShogiGame(MainScreen main, int seed) : base(main, seed, 9) { }
    }
}

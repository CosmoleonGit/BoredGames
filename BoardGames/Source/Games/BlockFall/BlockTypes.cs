using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.BlockFall
{
    internal static class BlockTypes
    {
        public static readonly int[][,] blocks = new int[][,]
        {
            // I
            new int[,]
            {
                {0, 0, 0, 0},
                {1, 1, 1, 1},
                {0, 0, 0, 0},
                {0, 0, 0, 0}
            },

            // J
            new int[,]
            {
                {2, 0, 0},
                {2, 2, 2},
                {0, 0, 0}
            },

            // L
            new int[,]
            {
                {0, 0, 3},
                {3, 3, 3},
                {0, 0, 0}
            },

            // O
            new int[,]
            {
                {4, 4},
                {4, 4},
            },

            // S
            new int[,]
            {
                {0, 5, 5},
                {5, 5, 0},
                {0, 0, 0}
            },

            // T
            new int[,]
            {
                {0, 6, 0},
                {6, 6, 6},
                {0, 0, 0}
            },

            // Z
            new int[,]
            {
                {7, 7, 0},
                {0, 7, 7},
                {0, 0, 0}
            },
        };

        public static int[,] PickRandom(Random rnd)
        {
            return blocks[rnd.Next(blocks.Length)];
        }
    }
}

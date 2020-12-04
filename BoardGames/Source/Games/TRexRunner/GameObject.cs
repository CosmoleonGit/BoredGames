using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.Games.TRexRunner
{
    public abstract class GameObject : Sprite
    {
        public bool CollidingWith(GameObject other) => Rectangle.Intersects(other.Rectangle);
    }
}

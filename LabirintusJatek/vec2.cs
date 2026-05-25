using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabirintusJatek
{
    internal class vec2
    {
        private int x;
        private int y;

        public vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static readonly vec2[] directions =
        {
            new vec2(0, -1), // Up
            new vec2(0, 1),  // Down
            new vec2(-1, 0), // Left
            new vec2(1, 0)   // Right
        };

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static vec2 operator +(vec2 a, vec2 b)
        {
            return new vec2(a.x + b.x, a.y + b.y);
        }
    }
}

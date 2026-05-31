using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabirintusJatek
{
    public class Vec2
    {
        private int x;
        private int y;

        public Vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static readonly Vec2[] directions =
        {
            new Vec2(0, -1), // Up
            new Vec2(0, 1),  // Down
            new Vec2(-1, 0), // Left
            new Vec2(1, 0)   // Right
        };

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Vec2 other)
            {
                return this.x == other.x && this.y == other.y;
            }
            return false;
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }
    }
}

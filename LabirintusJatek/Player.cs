using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabirintusJatek
{
    internal class Player
    {
        vec2 position;

        public vec2 Position { get => position;}

        

        public Player(vec2 position)
        {
            this.position = position;
        }

        public static vec2 DeterminePlayerPos(char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if(i == 0 || i == map.GetLength(0) - 1)
                    {
                        if(map[i, j] == '║')
                        {
                            return new vec2(j, i);
                        }
                    }

                    if (j == 0 || j == map.GetLength(1) - 1)
                    {
                        if (map[i, j] == '═')
                        {
                            return new vec2(j, i);
                        }
                    }
                }
            }
            throw new ArgumentException("Player position not found in the map.");
        }

        public bool Move(Direction dir, Map m)
        {
            vec2 newPos = position + vec2.directions[(int) dir];
            if (newPos.X < 0 || newPos.X >= m.Cols || newPos.Y < 0 || newPos.Y >= m.Rows)
            {
                return false; // Out of bounds
            }
            if (m[newPos.Y, newPos.X] != '.' && m.CheckDirections(position).Contains(dir))
            {
                position = newPos;
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LabirintusJatek
{
    internal class Player
    {
        vec2 position;
        int collectedTreasures = 0;
        public vec2 Position { get => position;}
        public int CollectedTreasures { get => collectedTreasures; set => collectedTreasures = value; }

        public Player(vec2 position)
        {
            this.position = position;
        }

        public Player()
        {
            position = new vec2(0, 0);
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

        public GameManager.MoveResult Move(Direction dir, Map m)
        {
            vec2 newPos = position + vec2.directions[(int) dir];
            if (newPos.X < 0 || newPos.X >= m.Cols || newPos.Y < 0 || newPos.Y >= m.Rows) // Out of bounds
            {
                if(collectedTreasures > 0)
                {
                    Console.WriteLine("Congratulations! You have collected all treasures and exited the labyrinth!");
                    return GameManager.MoveResult.EXITED;
                } 
                else 
                {
                    MessageBox.Show("You cannot exit the labyrinth without collecting any treasures!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return GameManager.MoveResult.INVALID_MOVE; 
                }
            }

            if (m[newPos.Y, newPos.X] != '.' && m.CheckDirections(position).Contains(dir))
            {
                position = newPos;
                return GameManager.MoveResult.VALID_MOVE;
            }

            return GameManager.MoveResult.INVALID_MOVE;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LabirintusJatek
{
    public class Player : INotifyPropertyChanged
    {
        Vec2 position;
        int collectedTreasures = 0;

        public Vec2 Position {
            get => position;
            set
            {
                position = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CollectedTreasures)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanExit)));
            }
        }
        public int CollectedTreasures {
            get => collectedTreasures;
            set
            {
                collectedTreasures = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CollectedTreasures)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanExit)));
            }
        }

        public bool CanExit => collectedTreasures > 0;

        public event PropertyChangedEventHandler? PropertyChanged;


        public Player(Vec2 position)
        {
            this.position = position;
        }

        public Player()
        {
            position = new Vec2(0, 0);
        }

        public override string ToString()
        {
            return $"{position};{collectedTreasures}";
        }

        public static Vec2 DeterminePlayerPos(char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if(i == 0 || i == map.GetLength(0) - 1)
                    {
                        if(map[i, j] == '║')
                        {
                            return new Vec2(j, i);
                        }
                    }

                    if (j == 0 || j == map.GetLength(1) - 1)
                    {
                        if (map[i, j] == '═')
                        {
                            return new Vec2(j, i);
                        }
                    }
                }
            }
            throw new ArgumentException("Player position not found in the map.");
        }

        public GameManager.MoveResult Move(Direction dir, Map m)
        {
            Vec2 newPos = position + Vec2.directions[(int) dir];
            if (isOutOfBounds(newPos, m) && m.CheckDirections(position).Contains(dir)) // Out of bounds
            {
                if(collectedTreasures > 0)
                {
                    return GameManager.MoveResult.EXITED;
                } 
                else 
                {
                    MessageBox.Show(Application.Current.Resources["ZeroTreasure"].ToString(), "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return GameManager.MoveResult.INVALID_MOVE; 
                }
            }

            if (!isOutOfBounds(newPos, m) && m[newPos.Y, newPos.X] != '.' && m.CheckDirections(position).Contains(dir) && m.GetInterConnections(position, dir).Contains(m[newPos.Y, newPos.X]))
            {
                Position = newPos;
                return GameManager.MoveResult.VALID_MOVE;
            }

            return GameManager.MoveResult.INVALID_MOVE;
        }

        private bool isOutOfBounds(Vec2 pos, Map m)
        {
            return pos.X < 0 || pos.X >= m.Cols || pos.Y < 0 || pos.Y >= m.Rows;
        }
    }
}

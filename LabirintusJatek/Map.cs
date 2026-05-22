using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace LabirintusJatek
{
    internal class Map
    {
        Player p;
        char[,] map;
        int rows;
        int cols;

        public int Rows { get => rows; }
        public int Cols { get => cols; }

        public Player P { get => p; }

        public char this[int i, int j]
        {
            get { return map[i, j]; }
        }

        public Map(Player p, char[,] map)
        {
            this.p = p;
            this.map = map;
            rows = map.GetLength(0);
            cols = map.GetLength(1);
        }

        public static bool CheckMapIntegrity(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Length != lines[0].Length)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

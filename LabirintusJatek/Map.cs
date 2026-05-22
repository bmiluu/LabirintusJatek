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
        char[,] map;
        int rows;
        int cols;

        public int Rows { get => rows; }
        public int Cols { get => cols; }

        public char this[int i, int j]
        {
            get { return map[i, j]; }
        }

        public Map( char[,] map)
        {
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

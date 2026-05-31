using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace LabirintusJatek
{
    public class Map
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

        public Map(char[,] map)
        {
            this.map = map;
            rows = map.GetLength(0);
            cols = map.GetLength(1);
        }

        public Map()
        {
            rows = 0; cols = 0;
            map = new char[rows, cols];
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

        public Direction[] CheckDirections(Vec2 pPos)
        {
            switch (map[pPos.Y, pPos.X])
            {
                case '═':
                    return [Direction.LEFT, Direction.RIGHT];

                case '║':
                    return [Direction.UP, Direction.DOWN];

                case '╔':
                    return [Direction.DOWN, Direction.RIGHT];

                case '╗':
                    return [Direction.DOWN, Direction.LEFT];

                case '╚':
                    return [Direction.UP, Direction.RIGHT];

                case '╝':
                    return [Direction.UP, Direction.LEFT];

                case '╦':
                    return [Direction.LEFT, Direction.RIGHT, Direction.DOWN];

                case '╩':
                    return [Direction.LEFT, Direction.RIGHT, Direction.UP];

                case '╣':
                    return [Direction.UP, Direction.DOWN, Direction.LEFT];

                case '╠':
                    return [Direction.UP, Direction.DOWN, Direction.RIGHT];

                case '╬':
                    return [Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT];

                case '█':
                    return [Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT];
            }
            return [];
        }

        public char[] GetInterConnections(Vec2 pos, Direction dir)
        {
            if (dir == Direction.UP)
            {
                if (new[] { '║', '╚', '╝', '╩', '╣', '╠', '╬', '█' }.Contains(map[pos.Y, pos.X]))
                    return ['║', '╔', '╗', '╦', '╣', '╠', '╬', '█'];
            }

            if (dir == Direction.DOWN)
            {
                if (new[] { '║', '╔', '╗', '╦', '╣', '╠', '╬', '█' }.Contains(map[pos.Y, pos.X]))
                    return ['║', '╚', '╝', '╩', '╣', '╠', '╬', '█'];
            }
            if (dir == Direction.LEFT)
            {
                if (new[] { '═', '╗', '╝', '╩', '╦', '╣', '╬', '█' }.Contains(map[pos.Y, pos.X]))
                    return ['═', '╔', '╚', '╦', '╩', '╠', '╬', '█'];
            }

            if(dir == Direction.RIGHT)
            {
                if (new[] { '═', '╔', '╚', '╩', '╦', '╠', '╬', '█' }.Contains(map[pos.Y, pos.X]))
                    return ['═', '╗', '╝', '╦', '╩', '╣', '╬', '█'];
            }
            return [];
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(map[i, j]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}

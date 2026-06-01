using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabirintusJatek.Util
{
    public static class Util
    {
        /// <summary>
        /// Megadja, hogy hány termet tartamaz a térkép
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Termek száma</returns>

        public static int GetRoomNumber(char[,] map)
        {
            int count = 0;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '█')
                        count++;
                }
            }

            return count;
        }
        private static bool IsPath(char c)
        {
            return c != '.' && c != '█';
        }

        /// <summary>
        /// A kapott térkép széleit végignézve megállapítja, hogy hány kijárat van.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Az alkalmas kijáratok száma</returns>
        public static int GetSuitableEntrance(char[,] map)
        {
            int count = 0;
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    bool border =
                        i == 0 ||
                        i == rows - 1 ||
                        j == 0 ||
                        j == cols - 1;

                    if (border && IsPath(map[i, j]))
                        count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Megnézi, hogy van-e a térképen meg nem engedett karakter?
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>true - A térkép tartalmaz szabálytalan karaktert, false - nincs benne ilyen</returns>
        public static bool IsInvalidElement(char[,] map)
        {
            char[] valid =
            {
                '.', '█',
                '║', '═',
                '╔', '╗',
                '╚', '╝',
                '╠', '╣',
                '╦', '╩',
                '╬'
            };

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (!valid.Contains(map[i, j]))
                        return true;
                }
            }

            return false;
        }

        private static bool ConnectsUp(char c)
        {
            return c == '║' || c == '╚' || c == '╝' ||
                   c == '╠' || c == '╣' || c == '╬';
        }

        private static bool ConnectsDown(char c)
        {
            return c == '║' || c == '╔' || c == '╗' ||
                   c == '╠' || c == '╣' || c == '╬';
        }

        private static bool ConnectsLeft(char c)
        {
            return c == '═' || c == '╗' || c == '╝' ||
                   c == '╦' || c == '╩' || c == '╬';
        }

        private static bool ConnectsRight(char c)
        {
            return c == '═' || c == '╔' || c == '╚' ||
                   c == '╦' || c == '╩' || c == '╬';
        }


        /// <summary>
        /// Visszaadja azoknak a járatkaraktereknek a pozícióját, amelyekhez egyetlen szomszéd pozícióból sem lehet eljutni.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>A pozíciók "sor_index:oszlop_index" formátumban szerepelnek a lista elemeiként
        public static List<string> GetUnavailableElements(char[,] map)
        {
            List<string> result = new List<string>();

            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (!IsPath(map[r, c]))
                        continue;

                    bool connected = false;

                    // fel
                    if (r > 0 &&
                        ConnectsUp(map[r, c]) &&
                        ConnectsDown(map[r - 1, c]))
                        connected = true;

                    // le
                    if (r < rows - 1 &&
                        ConnectsDown(map[r, c]) &&
                        ConnectsUp(map[r + 1, c]))
                        connected = true;

                    // bal
                    if (c > 0 &&
                        ConnectsLeft(map[r, c]) &&
                        ConnectsRight(map[r, c - 1]))
                        connected = true;

                    // jobb
                    if (c < cols - 1 &&
                        ConnectsRight(map[r, c]) &&
                        ConnectsLeft(map[r, c + 1]))
                        connected = true;

                    if (!connected)
                        result.Add($"{r}:{c}");
                }
            }

            return result;
        }


        /// <summary>
        /// Labiritust generál a kapott pozíciókat tartalmazó lista alapján. A lista elemei egymáshoz kapcsolódó járatok pozíciói.
        /// </summary>
        /// <param name="positionsList">"sor_index:oszlop_index" formátumban az egymáshoz kapcsolódó járatok pozícióit tartalmazó lista </param>
        /// <returns>A létrehozott labirintus térképe</returns>
        public static char[,] GenerateLabyrinth(List<string> positionsList)
        {
            int maxRow = 0;
            int maxCol = 0;

            HashSet<(int r, int c)> positions = new();

            foreach (string pos in positionsList)
            {
                string[] parts = pos.Split(':');

                int r = int.Parse(parts[0]);
                int c = int.Parse(parts[1]);

                positions.Add((r, c));

                maxRow = Math.Max(maxRow, r);
                maxCol = Math.Max(maxCol, c);
            }

            char[,] map = new char[maxRow + 1, maxCol + 1];

            for (int r = 0; r <= maxRow; r++)
            {
                for (int c = 0; c <= maxCol; c++)
                {
                    map[r, c] = '.';
                }
            }

            foreach (var p in positions)
            {
                bool up = positions.Contains((p.r - 1, p.c));
                bool down = positions.Contains((p.r + 1, p.c));
                bool left = positions.Contains((p.r, p.c - 1));
                bool right = positions.Contains((p.r, p.c + 1));

                if (up && down && left && right)
                    map[p.r, p.c] = '╬';
                else if (up && down && left)
                    map[p.r, p.c] = '╣';
                else if (up && down && right)
                    map[p.r, p.c] = '╠';
                else if (left && right && up)
                    map[p.r, p.c] = '╩';
                else if (left && right && down)
                    map[p.r, p.c] = '╦';
                else if (up && down)
                    map[p.r, p.c] = '║';
                else if (left && right)
                    map[p.r, p.c] = '═';
                else if (down && right)
                    map[p.r, p.c] = '╔';
                else if (down && left)
                    map[p.r, p.c] = '╗';
                else if (up && right)
                    map[p.r, p.c] = '╚';
                else if (up && left)
                    map[p.r, p.c] = '╝';
                else
                    map[p.r, p.c] = '•';
            }

            return map;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabirintusJatek.Util
{
    public static class Util
    {
        // ---------------------------
        // 1. ROOM SZÁMOLÁS (BFS)
        // ---------------------------

        /// <summary>
        /// Megadja, hogy hány termet tartamaz a térkép
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Termek száma</returns>

        static int GetRoomNumber(char[,] map)
        {
            int rows = map.GetLength(0); // sorok száma
            int cols = map.GetLength(1); // oszlopok száma

            bool[,] visited = new bool[rows, cols]; // jelöli, hogy jártunk-e már itt

            HashSet<char> walls = new HashSet<char>  // fal karakterek listája
            {
                '═','║','╔','╗','╚','╝','╦','╩','╣','╠','╬','█'
            };

            int rooms = 0; // itt számoljuk a különálló területeket

            // végigmegyünk minden mezőn
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (visited[i, j]) continue; // ha már bejártuk, kihagyjuk
                    if (walls.Contains(map[i, j])) continue; // fal nem számít

                    rooms++; // új összefüggő terület kezdődik

                    Queue<(int x, int y)> q = new(); // BFS sor
                    q.Enqueue((j, i)); // kiinduló pont
                    visited[i, j] = true; // megjelöljük

                    // BFS bejárás
                    while (q.Count > 0)
                    {
                        var (x, y) = q.Dequeue(); // aktuális mező

                        foreach (var d in Vec2.directions) // 4 irány
                        {
                            int nx = x + d.X; // új X koordináta
                            int ny = y + d.Y; // új Y koordináta

                            // határok ellenőrzése
                            if (nx < 0 || ny < 0 || nx >= cols || ny >= rows)
                                continue;

                            if (visited[ny, nx]) continue; // már jártunk itt
                            if (walls.Contains(map[ny, nx])) continue; // fal

                            visited[ny, nx] = true; // bejelöljük
                            q.Enqueue((nx, ny)); // tovább vizsgáljuk
                        }
                    }
                }
            }

            return rooms; // visszaadjuk hány terület van
        }

        // ---------------------------
        // 2. KIJÁRATOK SZÁMA
        // ---------------------------

        /// <summary>
        /// A kapott térkép széleit végignézve megállapítja, hogy hány kijárat van.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Az alkalmas kijáratok száma</returns>
        static int GetSuitableEntrance(char[,] map)
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            int count = 0; // kijáratok száma

            // felső és alsó sor vizsgálata
            for (int x = 0; x < cols; x++)
            {
                if (map[0, x] != '█') count++; // felső sor
                if (map[rows - 1, x] != '█') count++; // alsó sor
            }

            // bal és jobb oldal (sarok duplázás nélkül)
            for (int y = 1; y < rows - 1; y++)
            {
                if (map[y, 0] != '█') count++; // bal oldal
                if (map[y, cols - 1] != '█') count++; // jobb oldal
            }

            return count;
        }

        // ---------------------------
        // 3. ÉRVÉNYTELEN KARAKTER
        // ---------------------------

        /// <summary>
        /// Megnézi, hogy van-e a térképen meg nem engedett karakter?
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>true - A térkép tartalmaz szabálytalan karaktert, false - nincs benne ilyen</returns>
        static bool IsInvalidElement(char[,] map)
        {
            string valid = "═║╔╗╚╝╦╩╣╠╬█."; // engedélyezett karakterek

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (!valid.Contains(map[y, x])) // ha nem szerepel
                        return true; // hibás map
                }
            }

            return false; // minden ok
        }

        // ---------------------------
        // 4. ELÉRHETETLEN ELEMEK
        // ---------------------------

        /// <summary>
        /// Visszaadja azoknak a járatkaraktereknek a pozícióját, amelyekhez egyetlen szomszéd pozícióból sem lehet eljutni.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>A pozíciók "sor_index:oszlop_index" formátumban szerepelnek a lista elemeiként
        static List<string> GetUnavailableElements(char[,] map)
        {
            List<string> result = new(); // ide gyűjtjük az árva mezőket

            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (map[y, x] == '.') continue; // üres mező nem érdekes

                    bool hasNeighbour = false; // van-e szomszédja?

                    foreach (var d in Vec2.directions) // 4 irány
                    {
                        int nx = x + d.X;
                        int ny = y + d.Y;

                        // határok
                        if (nx < 0 || ny < 0 || nx >= cols || ny >= rows)
                            continue;

                        if (map[ny, nx] != '.') // ha van szomszéd
                        {
                            hasNeighbour = true;
                            break;
                        }
                    }

                    if (!hasNeighbour) // ha nincs kapcsolat
                        result.Add($"{y}:{x}"); // eltároljuk
                }
            }

            return result;
        }

        // ---------------------------
        // 5. LABIRINTUS GENERÁLÁS
        // ---------------------------

        /// <summary>
        /// Labiritust generál a kapott pozíciókat tartalmazó lista alapján. A lista elemei egymáshoz kapcsolódó járatok pozíciói.
        /// </summary>
        /// <param name="positionsList">"sor_index:oszlop_index" formátumban az egymáshoz kapcsolódó járatok pozícióit tartalmazó lista </param>
        /// <returns>A létrehozott labirintus térképe</returns>
        static char[,] GenerateLabyrinth(List<string> positionsList)
        {
            int maxR = 0; // legnagyobb sor
            int maxC = 0; // legnagyobb oszlop

            List<(int r, int c)> parsed = new(); // feldolgozott koordináták

            // string → koordináta
            foreach (var s in positionsList)
            {
                var parts = s.Split(':'); // "4:12"
                int r = int.Parse(parts[0]); // sor
                int c = int.Parse(parts[1]); // oszlop

                parsed.Add((r, c)); // eltároljuk

                maxR = Math.Max(maxR, r); // max sor
                maxC = Math.Max(maxC, c); // max oszlop
            }

            char[,] map = new char[maxR + 1, maxC + 1]; // pálya mérete

            // alap kitöltés (üres)
            for (int i = 0; i <= maxR; i++)
                for (int j = 0; j <= maxC; j++)
                    map[i, j] = '.';

            // bejárható pontok beírása
            foreach (var p in parsed)
                map[p.r, p.c] = '█';

            return map; // kész pálya
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LabirintusJatek
{
    public class GameManager
    {
        Map? m;
        Label[,] tiles;
        public static GameManager Instance { get; } = new GameManager();

        public void StartGame(string mapPath)
        {
            string[] lines = System.IO.File.ReadAllText(mapPath).Replace("\r", "").Split('\n');
            int rows = lines.Length;
            int cols = lines[0].Length;
            char[,] map = new char[rows, cols];
            if (Map.CheckMapIntegrity(lines))
            {
                // Proceed with game initialization
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        map[i, j] = lines[i][j];
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid map format. Please check the map file and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            vec2 pPos = Player.DeterminePlayerPos(map);
            m = new Map(new Player(pPos), map);
        }

        public void DrawMap(UniformGrid MazeGrid)
        {
            MazeGrid.Children.Clear();
            MazeGrid.Rows = m.Rows;
            MazeGrid.Columns = m.Cols;
            MazeGrid.Margin = new Thickness(0);

            tiles = new Label[m.Rows, m.Cols];

            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Cols; j++)
                {
                    Label tile = CreateTile(m[i, j]);
                    MazeGrid.Children.Add(tile);
                    tiles[i, j] = tile;
                }
            }

            tiles[m.P.Position.Y, m.P.Position.X].Background = Brushes.Yellow;
        }


        private Label CreateTile(char tileType)
        {
            Label tile = new Label();

            tile.Content = tileType.ToString();

            tile.FontSize = m.Cols*2;
            tile.FontWeight = FontWeights.Bold;

            tile.Foreground = Brushes.White;
            tile.Background = Brushes.Black;

            tile.FontFamily = new FontFamily("Consolas");

            tile.HorizontalAlignment = HorizontalAlignment.Center;
            tile.VerticalAlignment = VerticalAlignment.Center;

            tile.Margin = new Thickness(0);
            tile.Padding = new Thickness(0);
            tile.BorderThickness = new Thickness(0);

            return tile;
        }
    }
}

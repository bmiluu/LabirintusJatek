using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LabirintusJatek
{
    public class GameManager : INotifyPropertyChanged
    {
        private enum Modes
        {
            NORMAL,
            MIST
        }

        public enum MoveResult
        {
            VALID_MOVE,
            INVALID_MOVE,
            EXITED
        }

        Map m = new Map();
        Player p = new Player();

        public Player Player{
            get => p;
            set
            {
                p = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Player)));
            }
        }

        Label[,] tiles = new Label[0, 0];
        char[,] mistMap = new char[0, 0];

        Modes mode = Modes.MIST;

        List<Vec2> treasureRooms = new List<Vec2>();

        UniformGrid mGrid = null!;

        public event PropertyChangedEventHandler? PropertyChanged;

        public static GameManager Instance { get; } = new GameManager();
        
        public void StartGame(string mapPath)
        {
            string[] lines = File.ReadAllLines(mapPath);
            lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
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
                        if (lines[i][j] == '█')
                            treasureRooms.Add(new Vec2(j, i));
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid map format. Please check the map file and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Vec2 pPos = Player.DeterminePlayerPos(map);
            m = new Map(map);
            Player = new Player(pPos);
            mistMap = InitMistMap(rows, cols, '.');

            var result = MessageBox.Show("Do you want to play in Mist Mode? (Only adjacent tiles will be revealed)", "Choose Mode", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    mode = Modes.MIST;
                    break;
                case MessageBoxResult.No:
                    mode = Modes.NORMAL;
                    break;
            }
        }

        char[,] InitMistMap(int rows, int cols, char c)
        {
            char[,] mistMap = new char[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    mistMap[i, j] = c;
                }
            }
            return mistMap;
        }

        public void DrawMap(UniformGrid MazeGrid)
        {
            mGrid = MazeGrid;
            if (mode == Modes.NORMAL)
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

                DrawPlayer(p.Position);
            }
            if (mode == Modes.MIST)
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
                        Label tile = CreateTile(mistMap[i, j]);
                        MazeGrid.Children.Add(tile);
                        tiles[i, j] = tile;
                    }
                }
                UpdateMistMap(p.Position);
                DrawPlayer(p.Position);
            }
        }

        public void MovePlayer(Direction dir)
        {
            Vec2 oldPos = p.Position;
            switch (p.Move(dir, m))
            {
                case MoveResult.VALID_MOVE:
                    RedrawPlayer(p.Position, oldPos);
                    if (mode == Modes.MIST)
                    {
                        UpdateMistMap(p.Position);
                    }
                    break;

                case MoveResult.EXITED:
                    MessageBox.Show("Congratulations! You have exited the labyrinth!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    TriggerExitSequence();
                    break;

                case MoveResult.INVALID_MOVE:
                    break;
            }
        }


        private Label CreateTile(char tileType)
        {
            Label tile = new Label();

            tile.Content = tileType == '.' ? "" : tileType.ToString();

            tile.FontSize = m.Cols * 2;
            tile.FontWeight = FontWeights.Bold;

            tile.Foreground = Brushes.White;
            tile.Background = Brushes.Black;

            tile.FontFamily = new FontFamily("Consolas");

            tile.HorizontalAlignment = HorizontalAlignment.Stretch;
            tile.VerticalAlignment = VerticalAlignment.Stretch;

            tile.Margin = new Thickness(0);
            tile.Padding = new Thickness(0);
            tile.BorderThickness = new Thickness(0);

            return tile;
        }

        private void DrawPlayer(Vec2 pPos)
        {
            tiles[pPos.Y, pPos.X].Background = Brushes.Yellow;
            tiles[pPos.Y, pPos.X].Foreground = Brushes.Black;
        }

        private void RedrawPlayer(Vec2 pPos, Vec2 oldPos)
        {
            tiles[oldPos.Y, oldPos.X].Background = Brushes.Black;
            tiles[oldPos.Y, oldPos.X].Foreground = Brushes.White;
            tiles[pPos.Y, pPos.X].Background = Brushes.Yellow;
            tiles[pPos.Y, pPos.X].Foreground = Brushes.Black;
        }

        private void UpdateMistMap(Vec2 pPos)
        {
            mistMap[pPos.Y, pPos.X] = m[pPos.Y, pPos.X];
            tiles[pPos.Y, pPos.X].Content = m[pPos.Y, pPos.X] == '.' ? "" : m[pPos.Y, pPos.X].ToString();

            Direction[] directions = m.CheckDirections(pPos);

            foreach (var item in directions)
            {
                Vec2 newPos = pPos + Vec2.directions[(int)item];
                if (newPos.X >= mistMap.GetLength(1) || newPos.X < 0 || newPos.Y >= mistMap.GetLength(0) || newPos.Y < 0)
                {
                    continue;
                }
                mistMap[newPos.Y, newPos.X] = m[newPos.Y, newPos.X];
                tiles[newPos.Y, newPos.X].Content = m[newPos.Y, newPos.X] == '.' ? "" : m[newPos.Y, newPos.X].ToString();
            }
        }

        public void CollectTreasure()
        {
            if (m[p.Position.Y, p.Position.X] == '█' && treasureRooms.Contains(p.Position))
            {
                p.CollectedTreasures++;
                treasureRooms.Remove(p.Position);
            }
        }

        private void TriggerExitSequence()
        {
            MessageBox.Show($"You collected {p.CollectedTreasures} treasures!", "Exit Summary", MessageBoxButton.OK, MessageBoxImage.Information);
            mGrid.Children.Clear();
            Player = new Player();
            m = new Map();
            mistMap = new char[0, 0];
            tiles = new Label[0, 0];
        }

    }
}

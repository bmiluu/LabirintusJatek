using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public bool isGame = false;

        public Player Player
        {
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
                ShowError();
                return;
            }

            if(Util.Util.IsInvalidElement(map))
            {
                ShowError();
                return;
            }

            Vec2 pPos = Player.DeterminePlayerPos(map);

            if (pPos.Equals(new Vec2(-1, -1)))
            {
                ShowError();
                return;
            }
            
            m = new Map(map);
            Player = new Player(pPos);
            mistMap = InitMistMap(rows, cols, '.');

            var result = MessageBox.Show(Application.Current.Resources["ModeSelect"].ToString(), Application.Current.Resources["ChooseMode"].ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    mode = Modes.MIST;
                    break;
                case MessageBoxResult.No:
                    mode = Modes.NORMAL;
                    break;
            }
            isGame = true;
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
                    MessageBox.Show(Application.Current.Resources["ExitText"].ToString(), Application.Current.Resources["Success"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
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
            string template = Application.Current.Resources["ExitSummaryText"].ToString();
            string msg = string.Format(template, p.CollectedTreasures);
            MessageBox.Show(msg, Application.Current.Resources["ExitSummary"].ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            mGrid.Children.Clear();
            Player = new Player();
            m = new Map();
            mistMap = new char[0, 0];
            tiles = new Label[0, 0];
            isGame = false;
        }

        public void SaveMap(string savePath)
        {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < m.Rows; i++)
                {
                    for (int j = 0; j < m.Cols; j++)
                    {
                        sb.Append(m[i, j]);
                    }
                    sb.AppendLine();
                }

                sb.AppendLine();

                for (int i = 0; i < mistMap.GetLength(0); i++)
                {
                    for (int j = 0; j < mistMap.GetLength(1); j++)
                    {
                        sb.Append(mistMap[i, j]);
                    }
                    sb.AppendLine();
                }

                sb.AppendLine();
                
                sb.AppendLine(string.Join(";", treasureRooms));
                sb.AppendLine(mode.ToString());
                sb.AppendLine(p.ToString());
                File.WriteAllText(savePath, sb.ToString());
            
        }

        public void LoadMap(string loadPath)
        {
            treasureRooms.Clear();
            string[] lines = File.ReadAllLines(loadPath);

            int rows = lines.TakeWhile(line => !string.IsNullOrWhiteSpace(line)).Count();
            int cols = lines[0].Length;

            char[,] map = new char[rows, cols];
            char[,] mistMap = new char[rows, cols];

            string[] treasureRoomsStr = lines[lines.Length - 3].Split(';');
            string[] playerData = lines.Last().Split(';');

            string[] mapStr = lines.Take(rows).ToArray();
            string[] mistMapStr = lines.Skip(rows + 1).Take(rows).ToArray();

            FillMapFromStringArray(mapStr, map);
            FillMapFromStringArray(mistMapStr, mistMap);

            int collectedTreasures = int.Parse(playerData[1]);
            string posStr = playerData[0].Trim('(', ')');

            Player.Position = new Vec2(int.Parse(posStr.Split(',')[0]), int.Parse(posStr.Split(',')[1]));
            Player.CollectedTreasures = collectedTreasures;

            mode = Enum.Parse<Modes>(lines[lines.Length -2]);

            m = new Map(map);
            this.mistMap = mistMap;

            if (treasureRoomsStr[0] != "")
            {
                foreach (string treasureRoom in treasureRoomsStr)
                {
                    string trimmed = treasureRoom.Trim('(', ')');
                    treasureRooms.Add(new Vec2(int.Parse(trimmed.Split(',')[0]), int.Parse(trimmed.Split(',')[1])));
                }
            }

            isGame = true;
        }

        private void FillMapFromStringArray(string[] lines, char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = lines[i][j];
                }
            }
        }

        private void ShowError()
        {
            MessageBox.Show(Application.Current.Resources["MapError"].ToString(), Application.Current.Resources["Error"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static LabirintusJatek.MainWindow;

namespace LabirintusJatek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    //TODO 1: Check map integrity when loading in map
    //TODO 2: Change rendering to only display a 30x30 area around the player
    //TODO 3: Maybe add a minimap
    //TODO 4: Add win condition
    //TODO 5: Refactor code so it looks more nice
    public partial class MainWindow : Window
    {
        List<string> map = new List<string>();
        int[] pLoc = { 1, 0 };
        Border[,] tiles;

        public enum Direction
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Focus();
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Map Files (*.map)|*.map";
            if(ofd.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(ofd.FileName);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    map.Add(line);
                    MazeGrid.Columns = line.Length;
                }

                tiles = new Border[map.Count, map[0].Length];
                for (int x = 0; x < map.Count; x++)
                {
                    for(int y = 0; y < map[x].Length; y++)
                    {
                        Border tile = CreateTile(map[x][y]);
                        tiles[x,y] = tile;
                        MazeGrid.Children.Add(tile);
                    }
                }
                MazeGrid.LayoutTransform = new ScaleTransform(1.5, 1.5);
                tiles[pLoc[0], pLoc[1]].Background = Brushes.Yellow;
                ((TextBlock)tiles[pLoc[0], pLoc[1]].Child).Foreground = Brushes.Black;
            }
            
        }

        private Border CreateTile(char c)
        {
            return new Border
            {
                Background = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderThickness = new Thickness(0),
                Width = 20,
                Height = 20,
                SnapsToDevicePixels = true,
                Child = new TextBlock
                {
                    Text = c == '.' ? "" : c.ToString(),
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    MovePlayer(Direction.UP);
                    break;

                case Key.S:
                    MovePlayer(Direction.DOWN);
                    break;

                case Key.A:
                    MovePlayer(Direction.LEFT);
                    break;

                case Key.D:
                    MovePlayer(Direction.RIGHT);
                    break;
            }
        }

        void MovePlayer(Direction direction)
        {
            if (CheckForAllowedDirections().Contains(direction))
            {
                if (CheckIfValid(direction))
                {   
                    RedrawPlayer(direction);
                }

            }
        }

        Direction[] CheckForAllowedDirections()
        {
            switch (map[pLoc[0]][pLoc[1]])
            {
                case '═':
                    return new Direction[] {Direction.LEFT, Direction.RIGHT};

                case '║':
                    return new Direction[] {Direction.UP, Direction.DOWN};

                case '╔':
                    return new Direction[] { Direction.DOWN, Direction.RIGHT };

                case '╗':
                    return new Direction[] {Direction.DOWN, Direction.LEFT};

                case '╚':
                    return new Direction[] {Direction.UP,Direction.RIGHT};

                case '╝':
                    return new Direction[] { Direction.UP, Direction.LEFT };

                case '╦':
                    return new Direction[] { Direction.LEFT, Direction.RIGHT, Direction.DOWN };

                case '╩':
                    return new Direction[] { Direction.LEFT, Direction.RIGHT, Direction.UP };

                case '╣':
                    return new Direction[] {Direction.UP, Direction.DOWN, Direction.LEFT};

                case '╠':
                    return new Direction[] { Direction.UP, Direction.DOWN, Direction.RIGHT };

                case '╬':
                    return new Direction[] { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT};
            }
            return new Direction[] { };
        }

        bool CheckIfValid(Direction direction)
        {
            var (dx, dy) = GetDelta(direction);
            int newX = pLoc[0] + dx;
            int newY = pLoc[1] + dy;

            if (newX < 0 || newY < 0
                || newX >= map.Count
                || newY >= map[0].Length) return false;

            if (map[newX][newY] == '.') return false;
            
            return true;
        }

        (int dx, int dy) GetDelta(Direction direction)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    return (0, -1);

                case Direction.RIGHT:
                    return (0, 1);

                case Direction.UP:
                    return (-1, 0);

                case Direction.DOWN:
                    return (1, 0);

            }
            return (0, 0);
        }

        void RedrawPlayer(Direction direction)
        {
            tiles[pLoc[0], pLoc[1]].Background = Brushes.Black;
            ((TextBlock)tiles[pLoc[0], pLoc[1]].Child).Foreground = Brushes.White;
            var (dx, dy) = GetDelta(direction);
            pLoc[0] += dx;
            pLoc[1] += dy;
            tiles[pLoc[0], pLoc[1]].Background = Brushes.Yellow;
            ((TextBlock)tiles[pLoc[0], pLoc[1]].Child).Foreground = Brushes.Black;
        }
    }
}
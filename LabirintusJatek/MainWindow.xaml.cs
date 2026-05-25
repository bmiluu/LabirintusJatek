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

    //TODO 1: Check tile interconnections
    //TODO 2: Change rendering to only display a 30x30 area around the player
    //TODO 3: Maybe add a minimap
    //TODO 4: Add win condition
    //TODO 6: Change rendering format (image based)
    public partial class MainWindow : Window
    {
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
                GameManager.Instance.StartGame(ofd.FileName);
                MazeGrid.Children.Clear();
                GameManager.Instance.DrawMap(MazeGrid);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    GameManager.Instance.MovePlayer(Direction.UP);
                    break;

                case Key.S:
                    GameManager.Instance.MovePlayer(Direction.DOWN);
                    break;

                case Key.A:
                    GameManager.Instance.MovePlayer(Direction.LEFT);
                    break;

                case Key.D:
                    GameManager.Instance.MovePlayer(Direction.RIGHT);
                    break;
            }
        }
    }
}
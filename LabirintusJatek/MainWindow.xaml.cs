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

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = GameManager.Instance;
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Map Files (*.map)|*.map";
            if(ofd.ShowDialog() == true)
            {
                GameManager.Instance.StartGame(ofd.FileName);
                GameManager.Instance.DrawMap(MazeGrid);
            }
        }

        private void ReloadMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Map Files (*.SAV)|*.SAV";
            if(ofd.ShowDialog() == true)
            {
                GameManager.Instance.LoadMap(ofd.FileName);
                GameManager.Instance.DrawMap(MazeGrid);
            }
        }

        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.Instance.isGame)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Map Files (*.SAV)|*.SAV";
                if (sfd.ShowDialog() == true)
                {
                    GameManager.Instance.SaveMap(sfd.FileName);
                }
            }
            else
            {
                MessageBox.Show(Application.Current.Resources["NoGame"].ToString(), Application.Current.Resources["Error"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
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

                case Key.F:
                    GameManager.Instance.CollectTreasure();
                    break;

                case Key.G:
                    HideUIElements();
                    break;
            }
        }

        private void HideUIElements()
        {
            if(PlayerPositionLabel.Visibility == Visibility.Collapsed)
            {
                PlayerPositionLabel.Visibility = Visibility.Visible;
                TreasuresLabel.Visibility = Visibility.Visible;
                CanExitLabel.Visibility = Visibility.Visible;
                return;
            }

            PlayerPositionLabel.Visibility = Visibility.Collapsed;
            TreasuresLabel.Visibility = Visibility.Collapsed;
            CanExitLabel.Visibility = Visibility.Collapsed;
        }

        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LanguageSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string languageCode = selectedItem.Tag.ToString();
                LanguageManager.ChangeLanguage(languageCode);
            }
        }
    }
}
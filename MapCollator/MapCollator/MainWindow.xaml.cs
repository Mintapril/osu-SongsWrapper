using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;


namespace MapCollator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Botton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;

            }
            string path = folderDialog.SelectedPath.Trim();
            GlobalValue.path = PathBox.Text = path;

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string packName = PackNameBox.Text.ToString();
            string artists = ArtistsBox.Text.ToString();
            string creator = CreatorBox.Text.ToString();
            string OD = ODBox.Text.ToString();
            string HP = HPBox.Text.ToString();
            App.Program.Start(GlobalValue.path, packName, artists, creator, OD, HP);

        }
    }
}

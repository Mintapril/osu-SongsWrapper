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
using WPFFolderBrowser;


namespace MapCollator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string path;
        public MainWindow()
        {
            InitializeComponent();
            MutiThreading.MakeThreads();
        }

        public void Botton_Click(object sender, RoutedEventArgs e)
        {
            StructuralAnalysis.mainDict.Clear();
            StructuralAnalysis.opt.Clear();
            WPFFolderBrowserDialog wPFFolder = new WPFFolderBrowserDialog();

            wPFFolder.ShowDialog();
            try
            {
                path = wPFFolder.FileName;
            }
            catch (InvalidOperationException)
            {
                return;
            }
            GlobalValue.path = PathBox.Text = path;
            IO.GetFileList(path);
            StructuralAnalysis.AnalyzeStructure();
            foreach (var item in IO.allFileList)
            {
                if (item.Contains(".osu"))
                {
                    if (ListView.Items.Contains(StructuralAnalysis.mainDict[item][1]) == false)
                    {
                        ListView.Items.Add(StructuralAnalysis.mainDict[item][1] + "-" + StructuralAnalysis.mainDict[item][0] + " [" + StructuralAnalysis.mainDict[item][2] + "]");
                    }
                }
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string packName = PackNameBox.Text.ToString();
            string artists = ArtistsBox.Text.ToString();
            string creator = CreatorBox.Text.ToString();
            string OD = ODBox.Text.ToString();
            string HP = HPBox.Text.ToString();
            //检查用户是否选择了文件夹
            if (GlobalValue.path == null)
            {
                System.Windows.MessageBox.Show("Please select the file path!", "Error");
            }
            else
            {
                //检查hp和od的值是否合法
                if (double.TryParse(OD, out App.Program.od) == false)
                {
                    App.Program.ShowErrorMessageBox();
                }
                else if (App.Program.od > 10 || App.Program.od < 0)
                {
                    App.Program.ShowErrorMessageBox();
                }
                else
                {
                    if (double.TryParse(HP, out App.Program.hp) == false)
                    {
                        App.Program.ShowErrorMessageBox();
                    }
                    else if (App.Program.hp > 10 || App.Program.hp < 0)
                    {
                        App.Program.ShowErrorMessageBox();
                    }
                    else
                    {
                        MainWindow1.Title = "Processing......";
                        App.path = GlobalValue.path;
                        App.packName = packName;
                        App.artists = artists;
                        App.creator = creator;
                        App.OD = OD;
                        App.HP = HP;
                        App.Program.Start();
                        PathBox.Clear();
                        ListView.Items.Clear();
                        GlobalValue.path = null;
                        MainWindow1.Title = "MapCollator by mint";
                        MessageBox.Show("Packaged successfully!");
                        App.allFileDict.Clear();
                        App.allFileList.Clear();
                        StructuralAnalysis.mainDict.Clear();
                        StructuralAnalysis.opt.Clear();
                        IO.allFileList.Clear();
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ListView.Items.Clear();
            StructuralAnalysis.mainDict.Clear();
            StructuralAnalysis.opt.Clear();
            App.allFileDict.Clear();
            App.allFileList.Clear();
            IO.allFileList.Clear();
            PathBox.Clear();
            GlobalValue.path = null;
        }
    }
}

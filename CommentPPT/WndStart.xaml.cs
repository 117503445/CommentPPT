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
using System.IO;
using TLib.Software;
using PPt = Microsoft.Office.Interop.PowerPoint;
namespace CommentPPT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WndStart : Window
    {
        public WndStart()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = App.LstPaths.Count-1; i>=0; i--)
            {
                if (!File.Exists(App.LstPaths[i]))
                {
                    App.LstPaths.RemoveAt(i);
                }
            }
            LstPPT.ItemsSource = App.LstPaths;
        }
        private void BtnExplorer_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "PowerPoint Presentations|*.ppt;*.pptx"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (!App.LstPaths.Contains(openFileDialog.FileName))
                {
                    App.LstPaths.Add(openFileDialog.FileName);
                }
                OpenPPT(openFileDialog.FileName);
            }
        }
        /// <summary>
        /// 使用ppt打开指定path的文件
        /// </summary>
        /// <param name="path"></param>
        private void OpenPPT(string path)
        {
            var index = App.LstPaths.IndexOf(path);
            var temp = App.LstPaths[0];
            App.LstPaths[0] = App.LstPaths[index];
            App.LstPaths[index] = temp;

            System.Diagnostics.Process.Start(path);
        }

        private void LstPPT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenPPT((string)LstPPT.SelectedItem);
            
        }
    }
}

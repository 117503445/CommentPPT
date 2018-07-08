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
using System.Windows.Shapes;
using ppt = Microsoft.Office.Interop.PowerPoint;
using TLib.UI.WPF_MessageBox;
namespace CommentPPT
{
    /// <summary>
    /// WndPPT.xaml 的交互逻辑
    /// </summary>
    public partial class WndPPT : Window
    {
        /// <summary>
        /// 定义PowerPoint应用程序对象
        /// </summary>
        ppt.Application pptApplication;
        /// <summary>
        /// 定义演示文稿对象
        /// </summary>
        ppt.Presentation presentation;
        /// <summary>
        /// 定义幻灯片集合对象
        /// </summary>
        ppt.Slides slides;
        /// <summary>
        /// 定义单个幻灯片对象
        /// </summary>
        ppt.Slide slide;

        public WndPPT()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                pptApplication = PPT.TryGetApplication();
                if (pptApplication != null)
                {
                    break;
                }
                System.Threading.Thread.Sleep(2000);
            }
            if (pptApplication == null)
            {
                WdMessageBox.Display("错误", "即将关闭,请打开PPT再打开本软件");
                App.Current.Shutdown();
            }
        }
    }
}

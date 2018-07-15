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
using System.Windows.Threading;
using System.Reflection;

namespace CommentPPT
{
    /// <summary>
    /// WdPPT.xaml 的交互逻辑
    /// </summary>
    public partial class WdPPT : Window
    {
        /// <summary>
        /// 定义PowerPoint应用程序对象
        /// </summary>
        private ppt.Application pptApplication;
        /// <summary>
        /// 定义演示文稿对象
        /// </summary>
        private ppt.Presentation presentation;
        /// <summary>
        /// 定义幻灯片集合对象
        /// </summary>
        private ppt.Slides slides;
        /// <summary>
        /// 定义单个幻灯片对象
        /// </summary>
        private ppt.Slide slide;
        /// <summary>
        /// 幻灯片的数量
        /// </summary>
        private int slidesCount;
        /// <summary>
        /// 幻灯片的索引
        /// </summary>
        private int slideIndex;
        private int selectedIndex;

        private InkCanvas[] inks;

        private int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {

                var btns = new Button[] { BtnMouse, BtnPen, BtnEraser };
                var menus = new Control[] { BtnClearAll };
                foreach (var item in menus)
                {
                    item.Visibility = Visibility.Hidden;
                }
                for (int i = 0; i < btns.Length; i++)
                {
                    if (i == value)
                    {
                        btns[i].BorderThickness = new Thickness(1);
                    }
                    else
                    {
                        btns[i].BorderThickness = new Thickness(0);
                    }
                }
                if (value == selectedIndex)
                {
                    if (value == 1)
                    {
                        
                    }
                    else if (value == 2)
                    {
                        BtnClearAll.Visibility = Visibility.Visible;
                    }
                }
                UpdateInks();
                //UpdateSlideIndex();
                selectedIndex = value;
            }
        }
        public WdPPT()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 8; i++)
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

            //获得演示文稿对象
            presentation = pptApplication.ActivePresentation;
            // 获得幻灯片对象集合
            slides = presentation.Slides;
            // 获得幻灯片的数量
            slidesCount = slides.Count;

            inks = new InkCanvas[slidesCount];
            for (int i = 0; i < inks.Length; i++)
            {
                inks[i] = new InkCanvas
                {
                    Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)),

                };
                inks[i].SetValue(Grid.RowSpanProperty, 2);
                GMain.Children.Add(inks[i]);
            }
            Canvas.SetZIndex(GBottom, 1);
            DispatcherTimer dispatcherTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1), IsEnabled = true };
            dispatcherTimer.Tick += (sder, arg) =>
            {
                if (!IsPPtOpened())
                {
                    PPtClosed();
                    return;
                }
                UpdateSlideIndex();

                if (!IsPPtOpened())
                {
                    PPtClosed();
                    return;
                }
                try
                {
                    TbInfo.Text = $"name={presentation.Name}{Environment.NewLine}" +
               $"slideIndex={slideIndex}";
                    string s = slideIndex.ToString() + "/" + slidesCount.ToString();
                    LbLeft.Content = s;
                    LbRight.Content = s;

                }
                catch (Exception ex)
                {
                    TLib.Software.Logger.WriteException(ex);
                }
            };

            SelectedIndex = 0;//默认选择鼠标模式



        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private void ButtonUP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                slideIndex = slide.SlideIndex - 1;
            }
            catch
            {
                WdMessageBox.Display("Error", "PPt已关闭", "真是不幸");
                App.Current.Shutdown();
            }
            if (slideIndex >= 1)
            {
                try
                {
                    slide = slides[slideIndex];
                    slides[slideIndex].Select();
                }
                catch
                {
                    // 在阅读模式下使用下面的方式来切换到上一张幻灯片
                    pptApplication.SlideShowWindows[1].View.Previous();
                    slide = pptApplication.SlideShowWindows[1].View.Slide;
                }
            }
            else
            {
                slideIndex = 1;
                WdMessageBox.Display("Info", "已经到了第一页", "", "", "哦");
            }
            UpdateSlideIndex();
        }
        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                slideIndex = slide.SlideIndex + 1;
            }
            catch
            {
                WdMessageBox.Display("Error", "PPt已关闭", "真是不幸");
                App.Current.Shutdown();
            }
            if (slideIndex > slidesCount)
            {
                WdMessageBox.Display("Info", "已经到了最后一页", "", "", "哦");
                slideIndex = slidesCount;
            }
            else
            {
                try
                {
                    slide = slides[slideIndex];
                    slides[slideIndex].Select();
                }
                catch
                {
                    // 在阅读模式下使用下面的方式来切换到下一张幻灯片
                    pptApplication.SlideShowWindows[1].View.Next();
                    slide = pptApplication.SlideShowWindows[1].View.Slide;

                }
            }
            UpdateSlideIndex();
        }
        /// <summary>
        /// 刷新ppt的index
        /// </summary>
        private void UpdateSlideIndex()
        {
            try
            {
                // 在普通视图下这种方式可以获得当前选中的幻灯片对象
                // 然而在阅读模式下，这种方式会出现异常
                slide = slides[pptApplication.ActiveWindow.Selection.SlideRange.SlideNumber];
                //lastslide = slide;
                slideIndex = slide.SlideIndex;
            }
            catch
            {
                try
                {
                    // 在阅读模式下出现异常时，通过下面的方式来获得当前选中的幻灯片对象
                    slide = pptApplication.SlideShowWindows[1].View.Slide;
                    slideIndex = slide.SlideIndex;
                }
                catch
                {
                    Console.WriteLine("Fail UpdateSlideIndex");
                }

            }
            finally
            {
                UpdateInks();
            }

        }
        /// <summary>
        /// PPT还打开吗?
        /// </summary>
        private bool IsPPtOpened()
        {
            return PPT.TryGetApplication() != null;
        }
        /// <summary>
        /// PPT关闭以后,关闭程序
        /// </summary>
        private void PPtClosed()
        {
            //WdMessageBox.Display("Error", "PPt已关闭", "真是不幸");
            //MessageBox.Show("PPT已关闭");
            App.Current.Shutdown();
        }

        private void UpdateInks()
        {
            if (inks != null && inks[0] != null)//确认完成了初始化
            {
                for (int i = 0; i < inks.Length; i++)
                {

                    if (i == slideIndex && SelectedIndex != 0)
                    {
                        inks[i].Visibility = Visibility.Visible;
                        if (SelectedIndex == 1)
                        {
                            inks[i].EditingMode = InkCanvasEditingMode.Ink;
                        }
                        else if (SelectedIndex == 2)
                        {
                            inks[i].EditingMode = InkCanvasEditingMode.EraseByStroke;
                        }
                    }
                    else
                    {
                        inks[i].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void BtnMouse_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 0;
        }

        private void BtnPen_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 1;
        }

        private void BtnEraser_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 2;
        }
    }
}

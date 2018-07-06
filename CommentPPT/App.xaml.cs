using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TLib.Software;
namespace CommentPPT
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string Dir_APP { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string Dir_File { get; set; } = Dir_APP + "File/";
        
        public static string dir_LstPaths = Dir_File + "LstPaths.xml";
        public static BindingList<string> LstPaths { get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Directory.CreateDirectory(Dir_File);


            if (File.Exists(dir_LstPaths))
            {
                LstPaths = SerializeHelper.Load <BindingList<string> > (dir_LstPaths);
            }
            else
            {
                LstPaths = new BindingList<string>();
            }
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SerializeHelper.Save(LstPaths, dir_LstPaths);
        }


    }
}

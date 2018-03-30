using System;
using System.IO;
using System.Windows;

namespace JsonFileWatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, a) =>
            {
                File.WriteAllText(@"logs.txt", a.ExceptionObject.ToString());
            };
        }
    }
}

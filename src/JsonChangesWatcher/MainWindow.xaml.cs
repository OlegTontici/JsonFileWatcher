using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace JsonFileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileExtensionFilter = "(*.json) | *.json";
        private JsonView jsonView;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnSourceUpdate(string json)
        {
            jsonView.OnSourceUpdate(json);
        }

        private void ChooseFileButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = fileExtensionFilter
            };

            openFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                string fileName = openFileDialog.FileName;

                ChoosenPath.Text = fileName;

                jsonView = new JsonView(File.ReadAllText(fileName));
                RootContainer.Child = jsonView;

                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Directory.GetParent(fileName).FullName, new FileInfo(fileName).Name)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };

                fileSystemWatcher.Changed += (s, a) =>
                {
                    fileSystemWatcher.EnableRaisingEvents = false;

                    string data = string.Empty;

                    try
                    {
                        data = File.ReadAllText(fileName);
                    }
                    catch (Exception)
                    {
                        data = File.ReadAllText(fileName);
                    }

                    OnSourceUpdate(File.ReadAllText(fileName));

                    fileSystemWatcher.EnableRaisingEvents = true;
                };
            }
        }
    } 
}

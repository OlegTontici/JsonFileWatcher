using JsonFileWatcher.NodePresenters;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace JsonFileWatcher.FileJsonWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileExtensionFilter = "(*.json) | *.json";
        private JsonChangesObserver jsonSchemaChangesObserver;
        private IJsonUIFactory jsonUIFactory;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnSourceUpdate(string json)
        {
            jsonSchemaChangesObserver.OnSourceUpdate(json);
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

                string json = File.ReadAllText(fileName);
                jsonSchemaChangesObserver = new JsonChangesObserver(json, new JsonParser.JsonParser());
                jsonUIFactory = new JsonAsUITreeFactory(jsonSchemaChangesObserver.GetJsonAsObjectsTree());
                var jsonUI = jsonUIFactory.GetJsonAsUIElement();
                RootContainer.Child = jsonUI;

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

                    OnSourceUpdate(data);

                    fileSystemWatcher.EnableRaisingEvents = true;
                };
            }
        }
    } 
}

using System.Windows;

namespace JsonFileWatcher
{
    /// <summary>
    /// Interaction logic for SimpleWindow.xaml
    /// </summary>
    public partial class SimpleWindow : Window
    {
        private JsonView jsonView;
        public SimpleWindow()
        {
            InitializeComponent();
        }

        public void OnSourceUpdate(string json)
        {
            if(jsonView == null)
            {
                jsonView = new JsonView(json);
                RootContainer.Content = jsonView;
            }
            jsonView.OnSourceUpdate(json);
        }
    }
}

using JsonFileWatcher.NodePresenters;
using System.Windows;

namespace JsonFileWatcher.InMemoryJsonWatcher
{
    /// <summary>
    /// Interaction logic for SimpleWindow.xaml
    /// </summary>
    public partial class SimpleWindow : Window
    {
        private JsonChangesObserver jsonSchemaChangesObserver;
        private IJsonUIFactory jsonUIFactory;
        public SimpleWindow()
        {
            InitializeComponent();
        }

        public void OnSourceUpdate(string json)
        {
            if(jsonSchemaChangesObserver == null)
            {
                jsonSchemaChangesObserver = new JsonChangesObserver(json,new JsonParser.JsonParser());
                jsonUIFactory = new JsonAsUITreeFactory(jsonSchemaChangesObserver.GetJsonAsObjectsTree());
                var jsonUI = jsonUIFactory.GetJsonAsUIElement();
                RootContainer.Content = jsonUI;
            }
            jsonSchemaChangesObserver.OnSourceUpdate(json);
        }
    }
}

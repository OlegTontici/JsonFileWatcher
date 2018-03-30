using System.Windows.Controls;

namespace JsonFileWatcher.NodePresenters
{
    public class ObjectNode : CompositeNodeBase
    {
        private TextBlock open;
        private TextBlock close;
        
        public ObjectNode()
        {
            open = new TextBlock { Text = "{"};
            close = new TextBlock { Text = "}" };

            nodeContainer.Children.Add(open);
            nodeContainer.Children.Add(childContainer);
            nodeContainer.Children.Add(close);
        }
    }
}

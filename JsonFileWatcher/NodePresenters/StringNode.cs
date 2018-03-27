using System.Windows;
using System.Windows.Controls;

namespace JsonFileWatcher.NodePresenters
{
    public class StringNode : INode
    {
        private TextBlock propertyInfo;
        public StringNode(object propertyValue)
        {
            if(propertyValue != null)
            {
                propertyInfo = new TextBlock { Text = $"\"{propertyValue.ToString()}\"" };
            }   
        }

        public FrameworkElement GetNode()
        {
            return propertyInfo;
        }
    }
}

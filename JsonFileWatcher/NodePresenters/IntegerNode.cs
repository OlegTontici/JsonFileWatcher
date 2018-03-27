using System.Windows;
using System.Windows.Controls;

namespace JsonFileWatcher.NodePresenters
{
    public class IntegerNode : INode
    {
        private TextBlock propertyInfo;
        public IntegerNode(object propertyValue)
        {
            if (propertyValue != null)
            {
                propertyInfo = new TextBlock { Text = propertyValue.ToString() };
            }
        }
        
        public FrameworkElement GetNode()
        {
            return propertyInfo;
        }
    }
}

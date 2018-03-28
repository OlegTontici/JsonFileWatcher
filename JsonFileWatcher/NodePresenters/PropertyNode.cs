using System.Windows;
using System.Windows.Controls;

namespace JsonFileWatcher.NodePresenters
{
    public class PropertyNode : CompositeNodeBase
    {
        private TextBlock propertyInfo;

        public PropertyNode(string propertyName,object propertyValue)
        {
            propertyInfo = new TextBlock { Text = $"\"{propertyName}\" : " };

            if (propertyValue != null)
            {
                propertyInfo.Text += propertyValue;
            }

            nodeContainer.Children.Add(propertyInfo);
            nodeContainer.Children.Add(childContainer);
        }

        public override void AddChild(FrameworkElement child)
        {
            child.Margin = new Thickness(0, 0, 0, 0);
            childContainer.Children.Add(child);
        }
    }
}

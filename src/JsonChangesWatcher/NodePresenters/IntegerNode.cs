using JsonFileWatcher.Models;
using JsonFileWatcher.NodePresenters.NodesDecorators;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JsonFileWatcher.NodePresenters
{
    public class IntegerNode : INode
    {
        private TextBlock propertyInfo;
        public IntegerNode(ObjectNodeData node)
        {
            if (node.Value != null)
            {
                Binding b = new Binding("Value")
                {
                    Source = node
                };

                propertyInfo = new TextBlock { Background = Brushes.White };
                propertyInfo.SetBinding(TextBlock.TextProperty, b);

                node.PropertyChanged += new ChangedValueMarker(propertyInfo, "(TextBlock.Background).(SolidColorBrush.Color)").Animate;
            }
        }
        
        public FrameworkElement GetNode()
        {
            return propertyInfo;
        }
    }
}

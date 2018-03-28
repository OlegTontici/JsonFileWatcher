using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JsonFileWatcher.NodePresenters
{
    public class PropertyNode : CompositeNodeBase
    {
        private TextBlock propertyInfo;

        public PropertyNode(ObjectNodeData node)
        {
            propertyInfo = new TextBlock { Text = $"\"{node.Name}\" : " };

            if (node.Value != null)
            {
                Binding b = new Binding("Value")
                {
                    Source = node
                };

                TextBlock valueTb = new TextBlock { Background = Brushes.White };
                valueTb.SetBinding(TextBlock.TextProperty, b);

                propertyInfo.Inlines.Add(valueTb);

                node.PropertyChanged += new ChangedValueMarker(valueTb, "(TextBlock.Background).(SolidColorBrush.Color)").Animate;
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

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JsonFileWatcher.NodePresenters
{
    public class StringNode : INode
    {
        private TextBlock propertyInfo;
        public StringNode(ObjectNodeData node)
        {
            if(node != null)
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

    class ChangeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

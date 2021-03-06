﻿using JsonFileWatcher.Converters;
using JsonFileWatcher.Models;
using JsonFileWatcher.NodePresenters.NodesDecorators;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JsonFileWatcher.NodePresenters
{
    public class PropertyNode : CompositeNodeBase
    {
        private TextBlock propertyInfo;

        public PropertyNode(ObjectNodeData node) : base(node)
        {
            propertyInfo = new TextBlock { Text = $"\"{node.Name}\" : " };

            if (node.Value != null)
            {
                Binding b = new Binding("Value")
                {
                    Source = node,
                    Converter = new ObjectToFormatedStringConverter()
                };

                TextBlock valueTb = new TextBlock { Background = Brushes.White };
                valueTb.SetBinding(TextBlock.TextProperty, b);

                propertyInfo.Inlines.Add(valueTb);

                node.PropertyChanged += new ChangedValueMarker(valueTb, "(TextBlock.Background).(SolidColorBrush.Color)").Animate;
            }

            nodeContainer.Children.Add(propertyInfo);
            nodeContainer.Children.Add(childContainer);
        }

        public override FrameworkElement GetNode()
        {
            return nodeContainer;
        }

        protected override ItemsControl GetChildContainer()
        {
            return new ItemsControl
            {
                Height = double.NaN,
            };
        }
    }
}

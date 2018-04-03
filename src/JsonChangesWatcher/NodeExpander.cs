using JsonFileWatcher.NodePresenters;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JsonFileWatcher
{
    public class NodeExpander : StackPanel
    {
        private bool isExpanded = false;

        public EventHandler OnContentColapsed { get; set; }
        public EventHandler OnContentExpanded { get; set; }

        public NodeExpander(FrameworkElement node)
        {
            Orientation = Orientation.Horizontal;
            Button expandButton = new Button {
                Content = "-",
                Width = 20,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(-20, 0, 0, 0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };

            Children.Add(expandButton);
            Children.Add(node);

            expandButton.Click += (s, a) => 
            {
                if (!isExpanded)
                {
                    OnContentColapsed?.Invoke(this, null);
                    isExpanded = true;
                    expandButton.Content = "+";
                }
                else
                {
                    OnContentExpanded?.Invoke(this, null);
                    isExpanded = false;
                    expandButton.Content = "-";
                }
            };
        }
    }
}

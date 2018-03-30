using JsonFileWatcher.NodePresenters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JsonFileWatcher
{
    public class NodeExpander : StackPanel
    {
        private bool isExpanded = false;
        public NodeExpander(ICompositeNode node)
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
            Children.Add(node.GetNode());

            expandButton.Click += (s, a) => 
            {
                if (!isExpanded)
                {
                    node.HideContent();
                    isExpanded = true;
                    expandButton.Content = "+";
                }
                else
                {
                    node.ShowContent();
                    isExpanded = false;
                    expandButton.Content = "-";
                }
            };
        }
    }
}

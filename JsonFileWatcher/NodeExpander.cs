using JsonFileWatcher.NodePresenters;
using System.Windows;
using System.Windows.Controls;

namespace JsonFileWatcher
{
    public class NodeExpander : StackPanel
    {
        private bool isExpanded = false;
        public NodeExpander(ICompositeNode node)
        {
            Orientation = Orientation.Horizontal;
            Button expandButton = new Button {
                Style = (Style)FindResource("ExpanderButtonStyle"),
                Content = "-"
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

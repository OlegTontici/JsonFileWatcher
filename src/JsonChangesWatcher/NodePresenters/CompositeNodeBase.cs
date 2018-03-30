using System.Windows;
using System.Windows.Controls;

namespace JsonFileWatcher.NodePresenters
{
    public class CompositeNodeBase : ICompositeNode
    {
        private double childContainerHeight = 0;
        protected StackPanel nodeContainer;
        protected StackPanel childContainer;

        public CompositeNodeBase()
        {
            nodeContainer = new StackPanel();
            childContainer = new StackPanel { Height = double.NaN };
        }

        public virtual void AddChild(FrameworkElement child)
        {
            child.Margin = new Thickness(20, 0, 0, 0);
            childContainer.Children.Add(child);
        }

        public virtual FrameworkElement GetNode()
        {
            return nodeContainer;
        }

        public virtual void HideContent()
        {
            childContainerHeight = childContainer.Height;
            childContainer.Height = 0;
        }

        public virtual void ShowContent()
        {
            childContainer.Height = childContainerHeight;
        }
    }
}

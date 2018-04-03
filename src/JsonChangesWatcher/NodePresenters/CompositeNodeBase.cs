using JsonFileWatcher.Converters;
using JsonFileWatcher.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace JsonFileWatcher.NodePresenters
{
    public class CompositeNodeBase : ICompositeNode
    {
        private double childContainerHeight = 0;
        protected StackPanel nodeContainer;
        protected ItemsControl childContainer;

        public CompositeNodeBase(ObjectNodeData nodeData)
        {
            nodeContainer = new StackPanel();
            childContainer = new ItemsControl { Height = double.NaN };

            Binding binding = new Binding("Children")
            {
                Source = nodeData,
                Converter = new NodeDataToControlConverter()
            };

            childContainer.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        public virtual void AddChild(FrameworkElement child)
        {
            //child.Margin = new Thickness(20, 0, 0, 0);
            //childContainer.Children.Add(child);
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

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
            childContainer = GetChildContainer();

            Binding binding = new Binding("Children")
            {
                Source = nodeData,
                Converter = new NodeDataToControlConverter()
            };

            childContainer.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        public virtual FrameworkElement GetNode()
        {
            var expander = new NodeExpander(nodeContainer);

            expander.OnContentColapsed += (s, a) => HideContent();
            expander.OnContentExpanded += (s, a) => ShowContent();

            return expander;
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

        protected virtual ItemsControl GetChildContainer()
        {
            return new ItemsControl
            {
                Height = double.NaN,
                Margin = new Thickness(20, 0, 0, 0)
            };
        }
    }
}

using JsonFileWatcher.Converters;
using JsonFileWatcher.Models;
using JsonFileWatcher.NodePresenters.NodesDecorators;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace JsonFileWatcher.NodePresenters
{
    public class CompositeNodeBase : INode
    {
        private double childContainerHeight = 0;
        protected StackPanel nodeContainer;
        protected ItemsControl childContainer;

        public CompositeNodeBase(ObjectNodeData nodeData)
        {
            nodeContainer = new StackPanel();
            childContainer = GetChildContainer();
            childContainer.ItemsSource = nodeData.Children;
            SetContentTemplateFor(childContainer);
        }

        public virtual FrameworkElement GetNode()
        {
            var expander = new NodeExpander(nodeContainer);

            expander.OnContentColapsed += (s, a) => HideContent();
            expander.OnContentExpanded += (s, a) => ShowContent();

            return expander;
        }

        protected virtual void HideContent()
        {
            childContainerHeight = childContainer.Height;
            childContainer.Height = 0;
        }

        protected virtual void ShowContent()
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

        private void SetContentTemplateFor(ItemsControl itemsControl)
        {
            FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            frameworkElementFactory.SetBinding(ContentPresenter.ContentProperty, new Binding { Converter = new NodeDataToControlConverter() });
            DataTemplate dataTemplate = new DataTemplate { VisualTree = frameworkElementFactory };
            itemsControl.ItemTemplate = dataTemplate;
        }
    }
}

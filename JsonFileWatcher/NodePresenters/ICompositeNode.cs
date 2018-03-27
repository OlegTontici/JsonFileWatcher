using System.Windows;

namespace JsonFileWatcher.NodePresenters
{
    public interface ICompositeNode : INode
    {
        void AddChild(FrameworkElement child);
        void HideContent();
        void ShowContent();
    }
}

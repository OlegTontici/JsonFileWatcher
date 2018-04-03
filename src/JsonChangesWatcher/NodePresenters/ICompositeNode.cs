using System.Windows;

namespace JsonFileWatcher.NodePresenters
{
    public interface ICompositeNode : INode
    {
        void HideContent();
        void ShowContent();
    }
}

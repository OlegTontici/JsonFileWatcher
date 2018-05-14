using System.Windows;

namespace JsonFileWatcher
{
    public interface IJsonUIFactory
    {
        FrameworkElement GetJsonAsUIElement();
    }
}

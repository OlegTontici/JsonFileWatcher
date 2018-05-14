using JsonFileWatcher.Models;
using System.Windows;

namespace JsonFileWatcher.NodePresenters
{
    public class JsonAsUITreeFactory : IJsonUIFactory
    {
        ObjectNodeData _objectNodeData;
        public JsonAsUITreeFactory(ObjectNodeData objectNodeData)
        {
            _objectNodeData = objectNodeData;
        }

        public FrameworkElement GetJsonAsUIElement()
        {
            return new ObjectNode(_objectNodeData).GetNode();
        }
    }
}

using JsonFileWatcher.Models;

namespace JsonFileWatcher.JsonParser
{
    public interface IJsonParser
    {
        ObjectNodeData Parse(string json);
    }
}

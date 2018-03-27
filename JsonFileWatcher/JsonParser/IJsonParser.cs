namespace JsonFileWatcher.JsonParser
{
    public interface IJsonParser
    {
        ObjectNodeData Parse(string json);
    }
}

using Newtonsoft.Json.Linq;
using System;

namespace JsonFileWatcher.JsonParser
{
    public class JsonParser : IJsonParser
    {
        public ObjectNodeData Parse(string json)
        {
            bool isValidJson = false;
            JObject result = null;

            try
            {
                result = JObject.Parse(json);
                isValidJson = true;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Provided string is not Json formated");
            }

            if (isValidJson)
            {
                return GetNodes(result);
            }

            return new ObjectNodeData();
        }

        private ObjectNodeData GetNodes(JToken parrent)
        {
            ObjectNodeData node = new ObjectNodeData(parrent.Type)
            {
                Id = parrent.Path
            };

            if (parrent.Type == JTokenType.String || parrent.Type == JTokenType.Integer)
            {
                node.Value = parrent.Value<dynamic>().Value;
                return node;
            }

            if (parrent.Type == JTokenType.Property)
            {
                if (parrent.HasValues)
                {
                    node.Name = parrent.Value<dynamic>().Name;
                    foreach (var value in parrent.Children())
                    {
                        if (IsComposite(value.Type))
                        {
                            node.Children.Add(GetNodes(value));
                        }
                        else
                        {
                            node.Value = value.Value<dynamic>().Value;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in parrent.Children())
                {
                    node.Children.Add(GetNodes(item));
                }
            }

            return node;
        }

        private bool IsComposite(JTokenType type)
        {
            return type == JTokenType.Array || type == JTokenType.Object || type == JTokenType.Property;
        }
    }
}

using JsonFileWatcher.JsonParser;
using JsonFileWatcher.Models;
using System.Collections.Generic;
using System.Linq;

namespace JsonFileWatcher
{
    public class JsonChangesObserver
    {
        private Dictionary<string, ObjectNodeData> flattenObjectsTree;
        private IJsonParser _jsonParser;
        private ObjectNodeData objectsTree;

        public JsonChangesObserver(string json, IJsonParser jsonParser)
        {
            flattenObjectsTree = new Dictionary<string, ObjectNodeData>();
            _jsonParser = jsonParser;

            objectsTree = Parse(json);
            FlatObjectsTree(objectsTree, flattenObjectsTree);
        }

        public void OnSourceUpdate(string json)
        {
            ObjectNodeData newObjectsTree = Parse(json);
            Dictionary<string, ObjectNodeData> newFlattenObjectsTree = new Dictionary<string, ObjectNodeData>();
            FlatObjectsTree(newObjectsTree, newFlattenObjectsTree);

            if (!ObjectTreeHasChanged(newFlattenObjectsTree))
            {
                UpdateObjectTree(newFlattenObjectsTree, flattenObjectsTree);
            }            
        }

        public ObjectNodeData GetJsonAsObjectsTree()
        {
            return objectsTree;
        }

        private bool ObjectTreeHasChanged(Dictionary<string,ObjectNodeData> newFlattenData)
        {
            bool objectTreeWasChanged = false;
            var oldKeys = flattenObjectsTree.Keys.ToList();
            var AddedKeys = newFlattenData.Keys.Except(oldKeys);
            var RemovedKeys = oldKeys.Except(newFlattenData.Keys);

            if (AddedKeys.Any())
            {
                HashSet<string> addedItemsRoot = new HashSet<string>();

                foreach (var key in AddedKeys)
                {
                    string path = key;
                    while (!oldKeys.Contains(GetParrentPath(path)))
                    {
                        path = GetParrentPath(path);
                    }

                    addedItemsRoot.Add(path);

                    flattenObjectsTree.Add(key, newFlattenData[key]);
                }

                foreach (var path in addedItemsRoot)
                {
                    var newValue = newFlattenData[path];
                    var newValueParrent = newFlattenData[GetParrentPath(path)];
                    var parrentNode = flattenObjectsTree[GetParrentPath(path)];

                    int? newValuePosition = newValueParrent?.GetIndexFor(newValue);
                    parrentNode?.InsertChild(newValuePosition.Value, newValue);
                }

                objectTreeWasChanged = true;
            }

            if (RemovedKeys.Any())
            {
                HashSet<string> removedItemsRoot = new HashSet<string>();

                foreach (var key in RemovedKeys.ToList())
                {
                    string path = key;
                    while (!newFlattenData.Keys.Contains(GetParrentPath(path)))
                    {
                        path = GetParrentPath(path);
                    }

                    removedItemsRoot.Add(path);

                    flattenObjectsTree.Remove(key);
                }

                foreach (var path in removedItemsRoot)
                {
                    var parrentNode = flattenObjectsTree[GetParrentPath(path)];
                    var itemToRemove = parrentNode?.GetChildren(path);
                    parrentNode.RemoveChild(itemToRemove);
                }

                objectTreeWasChanged = true;
            }

            return objectTreeWasChanged;
        }

        private string GetParrentPath(string path)
        {
            if (path.EndsWith("]"))
            {
                return path.Substring(0, path.LastIndexOf('['));
            }
            return path.Substring(0, path.LastIndexOf('.'));
        }

        private ObjectNodeData Parse(string json)
        {
            return _jsonParser.Parse($"{{ \"data\" :{json} }}"); ;
        }
        private void FlatObjectsTree(ObjectNodeData node, Dictionary<string, ObjectNodeData> container)
        {
            foreach (var item in node.Children)
            {
                if (!container.ContainsKey(item.Id))
                {
                    container.Add(item.Id, item);
                }
                FlatObjectsTree(item, container);
            }
        }

        private void UpdateObjectTree(Dictionary<string, ObjectNodeData> newValue, Dictionary<string, ObjectNodeData> oldValue)
        {
            foreach (var item in newValue)
            {
                if (item.Value != null)
                {
                    ObjectNodeData oldProp = oldValue[item.Key];
                    ObjectNodeData newProp = item.Value;

                    if (oldProp != null && oldProp.Value != null && oldProp.Value.ToString() != newProp.Value.ToString())
                    {
                        oldProp.Value = newProp.Value;
                    }
                }
            }
        }
    }
}

﻿using JsonFileWatcher.Extensions;
using JsonFileWatcher.JsonParser;
using JsonFileWatcher.Models;
using JsonFileWatcher.NodePresenters;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Collections.Generic;

namespace JsonFileWatcher
{
    public class JsonView : Border
    {
        Dictionary<string,ObjectNodeData> flattenObjectsTree;
        private IJsonParser jsonParser;
        public JsonView(string json)
        {
            flattenObjectsTree = new Dictionary<string, ObjectNodeData>();
            jsonParser = new JsonParser.JsonParser();

            ObjectNodeData objectsTree = jsonParser.Parse($"{{ \"data\" :{json} }}");
            Child = CreateUiTree(objectsTree);

            FlatObjectsTree(objectsTree);
            RemoveChilds();
        }

        public void OnSourceUpdate(string json)
        {
            ObjectNodeData newObjectsTree = jsonParser.Parse($"{{ \"data\" :{json} }}");            

            if (ObjectTreeWasChanged(newObjectsTree))
            {
                return;
            }

            UpdateObjectTree(newObjectsTree, flattenObjectsTree);
        }

        private bool ObjectTreeWasChanged(ObjectNodeData newNode)
        {
            bool objectTreeWasChanged = false;
            var newFlattenData = GetFlattenContent(newNode);
            var oldKeys = flattenObjectsTree.Keys.ToList();

            var AddedKeys = newFlattenData.Keys.Except(oldKeys);
            var RemovedKeys = oldKeys.Except(newFlattenData.Keys);
            

            if (AddedKeys.Any())
            {
                ICollection<string> addedItemsRoot = new List<string>();

                foreach (var key in AddedKeys)
                {
                    string path = key;
                    while (!oldKeys.Contains(GetParrentPath(path)))
                    {
                        path = GetParrentPath(path);
                    }

                    if (!addedItemsRoot.Contains(path))
                    {
                        addedItemsRoot.Add(path);
                    }

                    flattenObjectsTree.Add(key, newFlattenData[key]);
                }

                foreach (var path in addedItemsRoot)
                {
                    var newValue = newFlattenData[path];
                    var parrentNode = flattenObjectsTree[GetParrentPath(path)];
                    Dispatcher.BeginInvoke(new Action(() => parrentNode.Children.FirstOrDefault()?.AddChild(newValue)));
                }
                objectTreeWasChanged = true;
            }

            if (RemovedKeys.Any())
            {
                ICollection<string> removedItemsRoot = new List<string>();

                foreach (var key in RemovedKeys.ToList())
                {
                    string path = key;
                    while (!newFlattenData.Keys.Contains(GetParrentPath(path)))
                    {
                        path = GetParrentPath(path);
                    }

                    if (!removedItemsRoot.Contains(path))
                    {
                        removedItemsRoot.Add(path);
                    }

                    flattenObjectsTree.Remove(key);
                }

                foreach (var path in removedItemsRoot)
                {
                    var parrentNodeChildren = flattenObjectsTree[GetParrentPath(path)].Children.FirstOrDefault()?.Children;
                    var itemToRemove = parrentNodeChildren.FirstOrDefaultRecursive(c => c.Id == path, c => c.Children);
                    Dispatcher.BeginInvoke(new Action(() => parrentNodeChildren.Remove(itemToRemove)));
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

        private void FlatObjectsTree(ObjectNodeData node)
        {
            foreach (var item in node.Children)
            {
                if (!flattenObjectsTree.ContainsKey(item.Id))
                {
                    flattenObjectsTree.Add(item.Id, item);
                }
                FlatObjectsTree(item);
            }
        }
        private Dictionary<string, ObjectNodeData> GetFlattenContent(ObjectNodeData nodeData)
        {
            Dictionary<string, ObjectNodeData> data = new Dictionary<string, ObjectNodeData>();

            foreach (var item in nodeData.Children)
            {
                if (!data.ContainsKey(item.Id))
                {
                    data.Add(item.Id, item);
                }

                foreach (var childItem in GetFlattenContent(item))
                {
                    if (!data.ContainsKey(childItem.Key))
                    {
                        data.Add(childItem.Key, childItem.Value);
                    }
                }
            }

            return data;
        }


        private void RemoveChilds()
        {
            //foreach (DictionaryEntry item in flattenObjectsTree)
            //{
            //    ((ObjectNodeData)item.Value).Children.Clear();
            //}
        }
        private FrameworkElement CreateUiTree(ObjectNodeData node)
        {
            return new ObjectNode(node).GetNode();
        }
        private void UpdateObjectTree(ObjectNodeData newValue, Dictionary<string, ObjectNodeData> oldValue)
        {
            foreach (var item in newValue.Children)
            {
                if (item.Value != null)
                {
                    ObjectNodeData oldPropValue = (ObjectNodeData)oldValue[item.Id];

                    if (oldPropValue != null && oldPropValue.Value != null && oldPropValue.Value.ToString() != item.Value.ToString())
                    {
                        oldPropValue.Value = item.Value;
                    }
                }
                else
                {
                    UpdateObjectTree(item, oldValue);
                }
            }
        }
    }

    class ObjectNodeDataComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> comparer;
        public ObjectNodeDataComparer(Func<T, T, bool> comparer)
        {
            this.comparer = comparer;
        }
        public bool Equals(T x, T y)
        {
            return comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
    class ObjectTreeModificationInfo
    {
        public IEnumerable<string> AddedKeys { get; set; }
        public IEnumerable<string> RemovedKeys { get; set; }

        public ObjectTreeModificationInfo()
        {
            AddedKeys = new List<string>();
            RemovedKeys = new List<string>();
        }

        public bool ModificationsOccured
        {
            get
            {
                return AddedKeys.Any() || RemovedKeys.Any();
            }
        }
    }
}

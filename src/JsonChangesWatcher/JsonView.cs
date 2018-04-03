using JsonFileWatcher.Extensions;
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
        Hashtable flattenObjectsTree;
        private IJsonParser jsonParser;
        public JsonView(string json)
        {
            flattenObjectsTree = new Hashtable();
            jsonParser = new JsonParser.JsonParser();

            ObjectNodeData objectsTree = jsonParser.Parse($"{{ \"data\" :{json} }}");
            Child = CreateUiTree(objectsTree);

            FlatObjectsTree(objectsTree);
            RemoveChilds();
        }

        public void OnSourceUpdate(string json)
        {
            ObjectNodeData newObjectsTree = jsonParser.Parse($"{{ \"data\" :{json} }}");

            ObjectTreeModificationInfo modificationInfo = GetObjectsTreeModificationInfo(newObjectsTree);

            if (modificationInfo.ModificationsOccured)
            {
                foreach (var key in modificationInfo.RemovedKeys.ToList())
                {
                    flattenObjectsTree.Remove(key);
                }

                Dispatcher.BeginInvoke(new Action(() => 
                {
                    Child = CreateUiTree(newObjectsTree);
                    FlatObjectsTree(newObjectsTree);
                    RemoveChilds();
                }));

                return;
            }

            UpdateObjectTree(newObjectsTree, flattenObjectsTree);
        }

        private ObjectTreeModificationInfo GetObjectsTreeModificationInfo(ObjectNodeData newNode)
        {
            ObjectTreeModificationInfo objectTreeModificationInfo = new ObjectTreeModificationInfo();

            var newKeys = newNode.Children.SelectManyRecursive(node => node.Children).Select( n => n.Id);
            var oldKeys = flattenObjectsTree.Keys.Cast<string>();

            objectTreeModificationInfo.AddedKeys = newKeys.Except(oldKeys);
            objectTreeModificationInfo.RemovedKeys = oldKeys.Except(newKeys);

            return objectTreeModificationInfo;
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

        private void RemoveChilds()
        {
            foreach (DictionaryEntry item in flattenObjectsTree)
            {
                ((ObjectNodeData)item.Value).Children.Clear();
            }
        }

        private FrameworkElement CreateUiTree(ObjectNodeData node)
        {
            INode nodePresenter = null;

            switch (node.Type)
            {
                case NodeType.Object:
                    nodePresenter = new ObjectNode(node);
                    break;
                case NodeType.String:
                    nodePresenter = new StringNode(node);
                    break;
                case NodeType.Integer:
                    nodePresenter = new IntegerNode(node);
                    break;
                case NodeType.Boolean:
                    break;
                case NodeType.Array:
                    nodePresenter = new ArrayNode(node);
                    break;
                case NodeType.Uri:
                    break;
                case NodeType.Timespan:
                    break;
                case NodeType.Date:
                    break;
                case NodeType.Float:
                    break;
                case NodeType.Null:
                    break;
                case NodeType.Property:
                    nodePresenter = new PropertyNode(node);
                    break;
                case NodeType.None:
                    break;
                default:
                    break;
            }

            return nodePresenter.GetNode();
        }
        private void UpdateObjectTree(ObjectNodeData newValue, Hashtable oldValue)
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

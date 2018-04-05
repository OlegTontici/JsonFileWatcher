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

                //Dispatcher.BeginInvoke(new Action(() => 
                //{
                //    Child = CreateUiTree(newObjectsTree);
                //    FlatObjectsTree(newObjectsTree);
                //    RemoveChilds();
                //}));

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

            List<string> addedRootNodes = new List<string>();
            List<string> removedRootNodes = new List<string>();

            foreach (var item in objectTreeModificationInfo.AddedKeys)
            {
                var asd = item.Substring(0, item.LastIndexOf('.'));
                if(objectTreeModificationInfo.AddedKeys.Contains(asd) && !addedRootNodes.Contains(asd))
                {
                    addedRootNodes.Add(asd.Substring(0, asd.LastIndexOf('[')));
                }
            }

            foreach (var item in objectTreeModificationInfo.RemovedKeys)
            {
                var asd = item.Substring(0, item.LastIndexOf('.'));
                if (objectTreeModificationInfo.RemovedKeys.Contains(asd) && !removedRootNodes.Contains(asd))
                {
                    removedRootNodes.Add(asd.Substring(0, asd.LastIndexOf('[')));
                }
            }

            var changedNode = addedRootNodes.FirstOrDefault();
            var newItemsForNode = newNode.Children.FirstOrDefaultRecursive(n => n.Id == changedNode,k => k.Children);
            var oldItemsForNode = flattenObjectsTree[changedNode];
            (oldItemsForNode as ObjectNodeData).AddChild(new ObjectNodeData(Newtonsoft.Json.Linq.JTokenType.String)
            {
                Id = changedNode + "[4]",
                Name = "NewNode",
                Value = "NewValue"
            });
            //var diff = newItemsForNode.Children[0].Children.Except((oldItemsForNode as ObjectNodeData).Children[0].Children,new ObjectNodeDataComparer<ObjectNodeData>((x,y) => x.Children[0].Value.ToString() != y.Children[0].Value.ToString()));
            //(oldItemsForNode as ObjectNodeData).Children[0].AddChild(diff.FirstOrDefault());

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
            //foreach (DictionaryEntry item in flattenObjectsTree)
            //{
            //    ((ObjectNodeData)item.Value).Children.Clear();
            //}
        }

        private FrameworkElement CreateUiTree(ObjectNodeData node)
        {
            return new ObjectNode(node).GetNode();
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

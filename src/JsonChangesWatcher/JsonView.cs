using JsonFileWatcher.JsonParser;
using JsonFileWatcher.Models;
using JsonFileWatcher.NodePresenters;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

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
            UpdateObjectTree(newObjectsTree, flattenObjectsTree);
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
                ((ObjectNodeData)item.Value).Children = null;
            }
        }

        private FrameworkElement CreateUiTree(ObjectNodeData node)
        {
            INode nodePresenter = null;

            switch (node.Type)
            {
                case NodeType.Object:
                    nodePresenter = new ObjectNode();
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
                    nodePresenter = new ArrayNode();
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

            foreach (var item in node.Children)
            {
                var child = CreateUiTree(item);
                if (child != null && nodePresenter != null)
                {
                    ((ICompositeNode)nodePresenter).AddChild(child);
                }
            };

            if (node.Type == NodeType.Object || node.Type == NodeType.Array)
                return new NodeExpander(((ICompositeNode)nodePresenter));

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
}

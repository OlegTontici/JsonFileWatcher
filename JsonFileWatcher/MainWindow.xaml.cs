using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json.Linq;
using JsonFileWatcher.NodePresenters;
using JsonFileWatcher.JsonParser;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace JsonFileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileExtensionFilter = "(*.json) | *.json";
        private IJsonParser jsonParser;
        private ObjectNodeData objectsTree;
        public MainWindow()
        {
            InitializeComponent();
            jsonParser = new JsonParser.JsonParser();
        }

        private void ChooseFileButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = fileExtensionFilter
            };

            openFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                string fileName = openFileDialog.FileName;

                ChoosenPath.Text = fileName;

                objectsTree = jsonParser.Parse($"{{ \"data\" :{File.ReadAllText(fileName)} }}");
                
                RootContainer.Child = CreateUiTree(objectsTree);

                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Directory.GetParent(fileName).FullName, new FileInfo(fileName).Name)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };

                fileSystemWatcher.Changed += (s, a) =>
                {
                    string data = string.Empty;

                    try
                    {
                        data = File.ReadAllText(fileName);
                    }
                    catch (Exception)
                    {
                        data = File.ReadAllText(fileName);
                    }

                    fileSystemWatcher.EnableRaisingEvents = false;

                    ObjectNodeData newObjectsTree = jsonParser.Parse($"{{ \"data\" :{File.ReadAllText(fileName)} }}");
                    UpdateObjectTree(newObjectsTree, objectsTree);

                    fileSystemWatcher.EnableRaisingEvents = true;
                };
            }
        }

        //handle ui representation creation
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

        private void UpdateObjectTree(ObjectNodeData newValue, ObjectNodeData oldValue)
        {
            foreach (var item in newValue.Children)
            {
                if(item.Value != null)
                {
                    var oldPropValue = oldValue.FirstOrDefaultRecursive(v => v.Id == item.Id);

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
    

    public class ObjectNodeData : INotifyPropertyChanged
    {
        private object _value;

        public string Id { get; set; }
        public NodeType Type { get; set; }
        public ObservableCollection<ObjectNodeData> Children { get; set; }
        public string Name { get; set; }
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }


        public ObjectNodeData(JTokenType type)
        {
            Children = new ObservableCollection<ObjectNodeData>();
            Type = GetNodeType(type);
        }
        public ObjectNodeData()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private NodeType GetNodeType(JTokenType type)
        {
            NodeType nodeType = NodeType.None;
            switch (type)
            {
                case JTokenType.Object:
                    nodeType = NodeType.Object;
                    break;
                case JTokenType.Array:
                    nodeType = NodeType.Array;
                    break;
                case JTokenType.Property:
                    nodeType = NodeType.Property;
                    break;
                case JTokenType.Comment:
                    break;
                case JTokenType.Integer:
                    nodeType = NodeType.Integer;
                    break;
                case JTokenType.Float:
                    break;
                case JTokenType.String:
                    nodeType = NodeType.String;
                    break;
                case JTokenType.Boolean:
                    nodeType = NodeType.Boolean;
                    break;
                case JTokenType.Null:
                case JTokenType.Undefined:
                    nodeType = NodeType.Null;
                    break;
                case JTokenType.Date:
                    nodeType = NodeType.Date;
                    break;
                case JTokenType.Uri:
                    nodeType = NodeType.Uri;
                    break;
                case JTokenType.TimeSpan:
                    nodeType = NodeType.Timespan;
                    break;
                default:
                    throw new ArgumentException();
            }

            return nodeType;
        }
        public ObjectNodeData FirstOrDefaultRecursive(Func<ObjectNodeData, bool> selector)
        {
            var found = selector(this);

            if (!found)
            {
                foreach (var item in this.Children)
                {
                    var result = item.FirstOrDefaultRecursive(selector);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else
            {
                return this;
            }
            return null;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum NodeType
    {
        Object,
        String,
        Integer,
        Boolean,
        Array,
        Uri,
        Timespan,
        Date,
        Float,
        Null,
        Property,
        None
    }
}

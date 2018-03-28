using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json.Linq;
using JsonFileWatcher.NodePresenters;
using JsonFileWatcher.JsonParser;
using System.Diagnostics;

namespace JsonFileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IJsonParser jsonParser;
        public MainWindow()
        {
            InitializeComponent();
            jsonParser = new JsonParser.JsonParser();
        }

        private void ChooseFileButtonClickEventHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "(*.json) | *.json"
            };

            openFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                string fileName = openFileDialog.FileName;

                ChoosenPath.Text = fileName;
                var tree = CreateUiTree(jsonParser.Parse(File.ReadAllText(fileName)));

                RootContainer.Child = tree;

                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Directory.GetParent(fileName).FullName, new FileInfo(fileName).Name)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };

                fileSystemWatcher.Changed += async (s, a) =>
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

                    await Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(()=>
                    {
                        Stopwatch stopwatch = new Stopwatch();

                        stopwatch.Start();
                        FrameworkElement newTree = CreateUiTree(jsonParser.Parse(data));
                       
                        RootContainer.Child = newTree;
                        stopwatch.Stop();

                    }));

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
                    nodePresenter = new StringNode(node.Value);
                    break;
                case NodeType.Integer:
                    nodePresenter = new IntegerNode(node.Value);
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
                    nodePresenter = new PropertyNode(node.Name, node.Value);
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
    }
    

    public class ObjectNodeData
    {
        public NodeType Type { get; set; }
        public List<ObjectNodeData> Children { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }


        public ObjectNodeData(JTokenType type)
        {
            Children = new List<ObjectNodeData>();
            Type = GetNodeType(type);
        }
        public ObjectNodeData()
        {

        }

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

            if(nodeType == NodeType.None)
            {
                string ad = "';";
            }
            return nodeType;
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

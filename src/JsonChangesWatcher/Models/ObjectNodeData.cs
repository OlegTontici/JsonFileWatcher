using JsonFileWatcher.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace JsonFileWatcher.Models
{
    public class ObjectNodeData : INotifyPropertyChanged
    {
        private object _value;
        private ObservableCollection<ObjectNodeData> _children;
        private Action<int, ObjectNodeData> insertChild;
        private Func<ObjectNodeData, int> getChildIndex;
        private Action<ObjectNodeData> removeChild;

        public string Id { get; set; }
        public NodeType Type { get; set; }
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
        public IReadOnlyCollection<ObjectNodeData> Children
        {
            get
            {
                return _children;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObjectNodeData(JTokenType type)
        {
            _children = new ObservableCollection<ObjectNodeData>();
            Type = GetNodeType(type);

            // TODO - get things below done right
            if(Type == NodeType.Property)
            {
                insertChild = (index,child) => _children.FirstOrDefault().InsertChild(index,child);
                getChildIndex = (child) => _children.FirstOrDefault().GetIndexFor(child);
                removeChild = (child) => _children.FirstOrDefault().RemoveChild(child);
            }
            else
            {
                insertChild = (index, child) => _children.Insert(index, child);
                getChildIndex = (child) => _children.IndexOf(child);
                removeChild = (child) => _children.Remove(child);
            }
        }
        public ObjectNodeData()
        {

        }

        public void AddChild(ObjectNodeData child)
        {
            _children.Add(child);
        }
        public void RemoveChild(ObjectNodeData child)
        {
            ExecuteOnDispatcherThread(() => removeChild(child));
        }

        public void InsertChild(int index,ObjectNodeData child)
        {
            ExecuteOnDispatcherThread(() => insertChild(index, child));
        }

        public int GetIndexFor(ObjectNodeData child)
        {
            return getChildIndex(child);
        }
        public ObjectNodeData GetChildren(string path)
        {
            return _children.FirstOrDefaultRecursive(c => c.Id == path, c => c.Children);
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

            return nodeType;
        }
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void ExecuteOnDispatcherThread(Action actionToExecute)
        {
            Application.Current.Dispatcher.BeginInvoke(actionToExecute);
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

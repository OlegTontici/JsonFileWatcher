using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace JsonFileWatcher.Models
{
    public class ObjectNodeData : INotifyPropertyChanged
    {
        private object _value;
        private ObservableCollection<ObjectNodeData> _children;

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
        public ObservableCollection<ObjectNodeData> Children
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
        }
        public ObjectNodeData()
        {

        }

        public void AddChild(ObjectNodeData child)
        {
            _children.Add(child);
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

﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace JsonFileWatcher.Models
{
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

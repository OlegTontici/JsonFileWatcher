using JsonFileWatcher.Models;
using JsonFileWatcher.NodePresenters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace JsonFileWatcher.Converters
{
    public class NodeDataToControlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetNode(value as ObjectNodeData).GetNode();
        }

        private INode GetNode(ObjectNodeData nodeData)
        {
            INode nodePresenter = null;

            switch (nodeData.Type)
            {
                case NodeType.Object:
                    nodePresenter = new ObjectNode(nodeData);
                    break;
                case NodeType.String:
                    nodePresenter = new StringNode(nodeData);
                    break;
                case NodeType.Integer:
                    nodePresenter = new IntegerNode(nodeData);
                    break;
                case NodeType.Boolean:
                    break;
                case NodeType.Array:
                    nodePresenter = new ArrayNode(nodeData);
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
                    nodePresenter = new PropertyNode(nodeData);
                    break;
                case NodeType.None:
                    break;
                default:
                    break;
            }
            return nodePresenter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

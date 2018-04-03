using JsonFileWatcher.Models;
using JsonFileWatcher.NodePresenters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace JsonFileWatcher.Converters
{
    public class NodeDataToControlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ObjectNodeData> children = value as ObservableCollection<ObjectNodeData>;
            ItemCollection itemCollection = new ListBox().Items;

            foreach (var node in children)
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
                    itemCollection.Add(nodePresenter.GetNode());
            }

            return itemCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

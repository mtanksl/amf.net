using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace mtanksl.ActionMessageFormat.FiddlerViewer
{
    public partial class AmfViewer : UserControl
    {
        public AmfViewer()
        {
            InitializeComponent();
        }

        private byte[] body;

        public byte[] Body
        {
            get
            {
                return body;
            }
            set
            {
                body = value;

                try
                {
                    var reader = new AmfReader(body);

                    var packet = reader.ReadAmfPacket();

                    treeListViewOutput.CanExpandGetter = o =>
                    {
                        return ( (Node)o ).Childs.Count > 0;
                    };

                    treeListViewOutput.ChildrenGetter = o =>
                    {
                        return ( (Node)o ).Childs;
                    };

                    treeListViewOutput.SetObjects(new[] {

                        Calculate("Packet", packet, packet.GetType().ToString() )
                    } );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Node Calculate(string name, object value, string type)
        {
            var parent = new Node() 
            {
                Name = name, 
                
                Type = type
            };

            if (value == null)
            {
                parent.Value = "null";
            }
            else if (value is char)
            {
                parent.Value = "'" + value + "'";
            }
            else if (value is string)
            {
                parent.Value = "\"" + value + "\"";
            }
            else if (value is ICollection collection)
            {
                parent.Value = "Count = " + collection.Count;
            }
            else
            {
                parent.Value = value.ToString();
            }

            if (value == null || value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is decimal || value is float || value is double || value is bool || value is string || value is DateTime || value is XmlDocument)
            {

            }
            else if (value is IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys)
                {
                    var item = dictionary[key];

                    var child = Calculate("[" + key + "]", item, item == null ? "" : item.GetType().ToString() );

                    parent.Childs.Add(child);
                }
            }
            else if (value is IEnumerable enumerable)
            {
                int i = 0;

                foreach (var item in enumerable)
                {
                    var child = Calculate("[" + i + "]", item, item == null ? "" : item.GetType().ToString() );

                    parent.Childs.Add(child);

                    i++;
                }
            }
            else
            {
                foreach (var property in value.GetType().GetProperties() )
                {
                    var child = Calculate(property.Name, property.GetValue(value), property.PropertyType.ToString() );

                    parent.Childs.Add(child);
                }

                var method = value.GetType().GetMethods().Where(m => m.Name == "ToObject" && m.GetParameters().Length == 0 && !m.IsGenericMethod).FirstOrDefault();

                if (method != null)
                {
                    method.Invoke(value, null);

                    var field = value.GetType().GetField("toObject", BindingFlags.Instance | BindingFlags.NonPublic);

                    if (field != null)
                    {
                        var child = Calculate(field.Name, field.GetValue(value), field.FieldType.ToString() );

                        parent.Childs.Add(child);
                    }
                }
            }

            return parent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeListViewOutput.ExpandAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeListViewOutput.CollapseAll();
        }
    }

    public class Node
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public List<Node> Childs { get; set; } = new List<Node>();
    }
}
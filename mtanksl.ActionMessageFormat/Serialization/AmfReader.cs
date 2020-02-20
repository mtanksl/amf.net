using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace mtanksl.ActionMessageFormat
{
    public class AmfReader
    {
        private byte[] data;

        private int offset;

        public AmfReader(byte[] data)
        {
            this.data = data;
        }

        private List<string> strings = new List<string>();

        private List<object> objects = new List<object>();

        private List<Amf3Trait> traits = new List<Amf3Trait>();

        public AmfPacket ReadAmfPacket()
        {
            var value = new AmfPacket()
            {
                Version = (AmfVersion)ReadInt16(),

                Headers = ReadAmfHeader(),

                Messages = ReadAmfMessage()
            };

            return value;
        }

        public List<AmfHeader> ReadAmfHeader()
        {
            var value = new List<AmfHeader>();

            short headerCount = ReadInt16();

            for (int i = 0; i < headerCount; i++)
            {
                string headerName = ReadAmf0String();

                bool mustUnderstand = ReadBoolean();

                int headerLength = ReadInt32();

                object data = ReadAmf0();

                value.Add(new AmfHeader()
                {
                    Name = headerName,

                    MustUnderstand = mustUnderstand,

                    Data = data
                } );
            }

            return value;
        }

        public List<AmfMessage> ReadAmfMessage()
        {
            var value = new List<AmfMessage>();

            short messageCount = ReadInt16();

            for (int i = 0; i < messageCount; i++)
            {
                string targetUri = ReadAmf0String();

                string responseUri = ReadAmf0String();
                
                int messageLength = ReadInt32();

                object data = ReadAmf0();

                value.Add(new AmfMessage()
                {
                    TargetUri = targetUri,

                    ResponseUri = responseUri,

                    Data = data 
                } );

                strings.Clear();

                objects.Clear();

                traits.Clear();
            }

            return value;
        }

        public byte ReadByte()
        {
            var value = data[offset];

            offset += 1;

            return value;
        }

        public bool ReadBoolean()
        {
            var value = BitConverter.ToBoolean(new[] { data[offset] }, 0);

            offset += 1;

            return value;
        }

        public short ReadInt16()
        {
            var value = BitConverter.ToInt16(new[] { data[offset + 1], data[offset] }, 0);

            offset += 2;

            return value;
        }

        public int ReadInt32()
        {
            var value = BitConverter.ToInt32(new[] { data[offset + 3], data[offset + 2], data[offset + 1], data[offset] }, 0);

            offset += 4;

            return value;
        }
        
        public double ReadDouble()
        {
            var value = BitConverter.ToDouble(new[] { data[offset + 7], data[offset + 6], data[offset + 5], data[offset + 4], data[offset + 3], data[offset + 2], data[offset + 1], data[offset] }, 0);

            offset += 8;

            return value;
        }

        public string ReadString(int length)
        {
            var value = new byte[length]; Buffer.BlockCopy(data, offset, value, 0, value.Length);

            offset += length;

            return Encoding.UTF8.GetString(value);
        }
        
        public object ReadAmf0()
        {
            var type = (Amf0Type)ReadByte();

            switch (type)
            {
                case Amf0Type.Number:

                    return ReadDouble();

                case Amf0Type.Boolean:

                    return ReadBoolean();

                case Amf0Type.String:

                    return ReadAmf0String(); 

                case Amf0Type.Object:

                    return ReadAmf0Object();

                case Amf0Type.Null:

                    break;

                case Amf0Type.Undefined:

                    break;

                case Amf0Type.Reference:

                    return ReadAmf0ObjectReference();

                case Amf0Type.EcmaArray:

                    return ReadAmf0Array();

                case Amf0Type.StrictArray:

                    return ReadAmf0StrictArray();

                case Amf0Type.Date:

                    return ReadAmf0Date();

                case Amf0Type.LongString:

                    return ReadAmf0LongString();

                case Amf0Type.Unsuported:

                    break;

                case Amf0Type.XMLDocument:

                    return ReadAmf0XmlDocument();

                case Amf0Type.TypedObject:

                    return ReadAmf0TypedObject();

                case Amf0Type.Amf3:

                    return ReadAmf3();
            }

            return null;
        }

        public string ReadAmf0String()
        {
            short length = ReadInt16();

            return ReadString(length);
        }

        public Amf0Object ReadAmf0Object()
        {
            var value = new Amf0Object() { DynamicMembersAndValues = new Dictionary<string, object>() };

            objects.Add(value);

            while (true)
            {
                string key = ReadAmf0String();

                if (key.Length == 0)
                {
                    break;
                }

                object data = ReadAmf0();

                value.DynamicMembersAndValues.Add(key, data);
            }

            offset += 1;

            return value;
        }

        public Amf0Object ReadAmf0ObjectReference()
        {
            int reference = ReadInt32();

            return (Amf0Object)objects[reference];
        }

        public Dictionary<string, object> ReadAmf0Array()
        {
            var value = new Dictionary<string, object>();

            int length = ReadInt32();

            for (int i = 0; i < length; i++)
            {
                string key = ReadAmf0String();

                object data = ReadAmf0();

                value.Add(key, data);
            }

            return value;
        }

        public List<object> ReadAmf0StrictArray()
        {
            var value = new List<object>();

            int length = ReadInt32();

            for (int i = 0; i < length; i++)
            {
                object data = ReadAmf0();

                value.Add(data);
            }

            return value;
        }

        public DateTime ReadAmf0Date()
        {
            double milliseconds = ReadDouble();

            short timeZone = ReadInt16();

            var value = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(milliseconds);

            return value;
        }

        public string ReadAmf0LongString()
        {
            int length = ReadInt32();

            return ReadString(length);
        }

        public XmlDocument ReadAmf0XmlDocument()
        {
            string xml = ReadAmf0LongString();

            var value = new XmlDocument();

            value.LoadXml(xml);

            return value;
        }

        public Amf0Object ReadAmf0TypedObject()
        {
            string className = ReadAmf0String();

            var value = new Amf0Object() { ClassName = className, DynamicMembersAndValues = new Dictionary<string, object>() };

            objects.Add(value);

            while (true)
            {
                string key = ReadAmf0String();

                if (key.Length == 0)
                {
                    break;
                }

                object data = ReadAmf0();

                value.DynamicMembersAndValues.Add(key, data);
            }

            offset += 1;

            return value;
        }

        public object ReadAmf3()
        {
            var type = (Amf3Type)ReadByte();

            switch (type)
            {
                case Amf3Type.Undefined:

                    break;

                case Amf3Type.Null:

                    break;

                case Amf3Type.BooleanFalse:

                    return false;

                case Amf3Type.BooleanTrue:

                    return true;

                case Amf3Type.Integer:

                    return ReadAmf3Int32();

                case Amf3Type.Double:

                    return ReadDouble();

                case Amf3Type.String:

                    return ReadAmf3String();

                case Amf3Type.XmlDocument:

                    return ReadAmf3XmlDocument();

                case Amf3Type.Date:

                    return ReadAmf3Date();

                case Amf3Type.Array:

                    return ReadAmf3Array();

                case Amf3Type.Object:

                    return ReadAmf3Object();

                case Amf3Type.Xml:

                    return ReadAmf3XmlDocument();

                case Amf3Type.ByteArray:

                    return ReadAmf3ByteArray();

                case Amf3Type.VectorInt:
                case Amf3Type.VectorUInt:

                    return ReadAmf3Int32Array();

                case Amf3Type.VectorDouble:

                    return ReadAmf3DoubleArray();

                case Amf3Type.VectorObject:

                    return ReadAmf3ObjectArray();

                case Amf3Type.Dictionary:

                    return ReadAmf3Dictionary();
            }

            return null;
        }

        public int ReadAmf3Int32()
        {
            byte valueA = ReadByte();

            if (valueA < 128)
            {
                return valueA;
            }

            byte valueB = ReadByte();

            if (valueB < 128)
            {
                return (valueA & 0x7F) << 7 | 
                       valueB;
            }

            byte valueC = ReadByte();

            if (valueC < 128)
            {
                return (valueA & 0x7F) << 14 | 
                       (valueB & 0x7F) << 7  | 
                       valueC;
            }

            byte valueD = ReadByte();

            return (valueA & 0x7F) << 22 | 
                   (valueB & 0x7F) << 15 |
                   (valueC & 0x7F) << 8  | 
                   valueD;
        }

        public string ReadAmf3String()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                var value = ReadString(length);

                strings.Add(value);

                return value;
            }

            return strings[reference >> 1];
        }

        public XmlDocument ReadAmf3XmlDocument()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                string xml = ReadString(length);

                var value = new XmlDocument();

                value.LoadXml(xml);

                objects.Add(value);

                return value;
            }

            return (XmlDocument)objects[reference >> 1];
        }

        public DateTime ReadAmf3Date()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                double milliseconds = ReadDouble();

                var value = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(milliseconds);

                objects.Add(value);

                return value;
            }

            return (DateTime)objects[reference >> 1];
        }

        public Amf3Array ReadAmf3Array()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                var value = new Amf3Array() { StrictDense = new List<object>(), SparseAssociative = new Dictionary<string, object>() };

                objects.Add(value);

                while (true)
                {
                    string key = ReadAmf3String();

                    if (key.Length == 0)
                    {
                        break;
                    }

                    object data = ReadAmf3();

                    value.SparseAssociative.Add(key, data);
                }
                
                for (int i = 0; i < length; i++)
                {
                    object data = ReadAmf3();

                    value.StrictDense.Add(data);
                }

                return value;
            }

            return (Amf3Array)objects[reference >> 1];
        }

        public Amf3Object ReadAmf3Object()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                reference = reference >> 1;

                Amf3Trait trait;

                if ( (reference & 0x01) == 0x01)
                {
                    reference = reference >> 1;

                    bool isExternalizable = (reference & 0x01) == 0x01;

                    reference = reference >> 1;

                    bool isDynamic = (reference & 0x01) == 0x01;

                    int length = reference >> 1;

                    string name = ReadAmf3String();

                    trait = new Amf3Trait()
                    {
                        ClassName = name,

                        IsDynamic = isDynamic,

                        IsExternalizable = isExternalizable,

                        Members = new List<string>()
                    };

                    traits.Add(trait);

                    for (int i = 0; i < length; i++)
                    {
                        string member = ReadAmf3String();

                        trait.Members.Add(member);
                    }
                }
                else
                {
                    trait = traits[reference >> 1];
                }

                var value = new Amf3Object()
                {
                    Trait = trait,

                    Values = new List<object>(),

                    DynamicMembersAndValues = new Dictionary<string, object>()
                };

                objects.Add(value);
                
                if (trait.IsExternalizable)
                {
                    if (value.Trait.ClassName == "flex.messaging.io.ArrayCollection" || value.Trait.ClassName == "flex.messaging.io.ObjectProxy")
                    {
                        value.Values.Add( ReadAmf3() );
                    }
                    else
                    {
                        var value2 = ( (IExternalizable)value.ToObject() );

                        value2.Read(this);

                        var value3 = new Amf3Object();

                        value3.FromObject(value2);

                        value.Values = value3.Values;

                        value.DynamicMembersAndValues = value3.DynamicMembersAndValues;
                    }
                }
                else
                {
                    for (int i = 0; i < trait.Members.Count; i++)
                    {
                        object data = ReadAmf3();

                        value.Values.Add(data);
                    }

                    if (trait.IsDynamic)
                    {
                        while (true)
                        {
                            string key = ReadAmf3String();

                            if (key.Length == 0)
                            {
                                break;
                            }

                            object data = ReadAmf3();

                            value.DynamicMembersAndValues.Add(key, data);
                        }
                    }
                }

                return value;
            }

            return (Amf3Object)objects[reference >> 1];
        }

        public List<byte> ReadFlags()
        {
            var flags = new List<byte>();

            while (true)
            {
                byte flag = ReadByte();

                flags.Add(flag);

                if ( (flag & 128) == 0)
                {
                    break;
                }
            }

            return flags;
        }

        public List<byte> ReadAmf3ByteArray()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                var value = new List<byte>();

                objects.Add(value);
                
                for (int i = 0; i < length; i++)
                {
                    byte data = ReadByte();

                    value.Add(data);
                }

                return value;
            }

            return ( List<byte> )objects[reference >> 1];
        }

        public List<int> ReadAmf3Int32Array()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                bool fixedVector = ReadBoolean();

                var value = new List<int>();

                objects.Add(value);
                
                for (int i = 0; i < length; i++)
                {
                    int data = ReadAmf3Int32();

                    value.Add(data);
                }

                return value;
            }

            return ( List<int> )objects[reference >> 1];
        }

        public List<double> ReadAmf3DoubleArray()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                bool fixedVector = ReadBoolean();

                var value = new List<double>();

                objects.Add(value);
                
                for (int i = 0; i < length; i++)
                {
                    double data = ReadDouble();

                    value.Add(data);
                }

                return value;
            }

            return ( List<double> )objects[reference >> 1];
        }

        public List<object> ReadAmf3ObjectArray()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                bool fixedVector = ReadBoolean();

                string objectTypeName = ReadAmf3String();

                var value = new List<object>();

                objects.Add(value);
                
                for (int i = 0; i < length; i++)
                {
                    object data = ReadAmf3();

                    value.Add(data);
                }

                return value;
            }

            return ( List<object> )objects[reference >> 1];
        }

        public Dictionary<object, object> ReadAmf3Dictionary()
        {
            int reference = ReadAmf3Int32();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                bool weakKeys = ReadBoolean();

                var value = new Dictionary<object, object>();

                objects.Add(value);

                for (int i = 0; i < length; i++)
                {
                    object key = ReadAmf3();

                    object data = ReadAmf3();

                    value.Add(key, data);
                }

                return value;
            }

            return ( Dictionary<object, object> )objects[reference >> 1];
        }
    }
}
﻿using System;
using System.Collections.Generic;
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
        
        public bool CanReadByte()
        {
            return offset < data.Length;
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

        public ushort ReadUInt16()
        {
            var value = BitConverter.ToUInt16(new[] { data[offset + 1], data[offset] }, 0);

            offset += 2;

            return value;
        }

        public int ReadInt32()
        {
            var value = BitConverter.ToInt32(new[] { data[offset + 3], data[offset + 2], data[offset + 1], data[offset] }, 0);

            offset += 4;

            return value;
        }

        public uint ReadUInt32()
        {
            var value = BitConverter.ToUInt32(new[] { data[offset + 3], data[offset + 2], data[offset + 1], data[offset] }, 0);

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

                strings.Clear();

                objects.Clear();

                traits.Clear();
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

                    return null;

                case Amf0Type.Undefined:

                    return null;

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

                    return null;

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

        public string ReadAmf0LongString()
        {
            int length = ReadInt32();

            return ReadString(length);
        }

        public Amf0Object ReadAmf0Object()
        {
            var value = new Amf0Object() 
            {
                ClassName = "", 
                
                DynamicMembersAndValues = new Dictionary<string, object>()
            };

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

        public Amf0Object ReadAmf0TypedObject()
        {
            string className = ReadAmf0String();

            var value = new Amf0Object() 
            {
                ClassName = className, 
                
                DynamicMembersAndValues = new Dictionary<string, object>() 
            };

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

        public XmlDocument ReadAmf0XmlDocument()
        {
            string xml = ReadAmf0LongString();

            var value = new XmlDocument();

            value.LoadXml(xml);

            return value;
        }

        public object ReadAmf3()
        {
            var type = (Amf3Type)ReadByte();

            switch (type)
            {
                case Amf3Type.Undefined:

                    return null;

                case Amf3Type.Null:

                    return null;

                case Amf3Type.BooleanFalse:

                    return false;

                case Amf3Type.BooleanTrue:

                    return true;

                case Amf3Type.Integer:

                    return ReadAmf3UInt29();

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

                    return ReadAmf3Int32List();

                case Amf3Type.VectorUInt:

                    return ReadAmf3UInt32List();

                case Amf3Type.VectorDouble:

                    return ReadAmf3DoubleList();

                case Amf3Type.VectorObject:

                    return ReadAmf3ObjectList();

                case Amf3Type.Dictionary:

                    return ReadAmf3Dictionary();
            }

            return null;
        }

        public List<byte> ReadFlags()
        {
            var flags = new List<byte>();

            while (true)
            {
                byte flag = ReadByte();

                flags.Add(flag);

                if ( (flag & 0x80) == 0x00)
                {
                    break;
                }
            }

            return flags;
        }

        public int ReadAmf3UInt29()
        {
            byte valueA = ReadByte();

            if (valueA <= 0x7F)
            {
                return valueA;
            }

            byte valueB = ReadByte();

            if (valueB <= 0x7F)
            {
                return (valueA & 0x7F) << 7 | valueB;
            }

            byte valueC = ReadByte();

            if (valueC <= 0x7F)
            {
                return (valueA & 0x7F) << 14 | (valueB & 0x7F) << 7 | valueC;
            }

            byte valueD = ReadByte();

            int ret = (valueA & 0x7F) << 22 | (valueB & 0x7F) << 15 | (valueC & 0x7F) << 8 | valueD;
            
            if ( (ret & 268435456) == 268435456)
            {
                ret = ret | -536870912;
            }

            return ret;
        }

        public string ReadAmf3String()
        {
            int reference = ReadAmf3UInt29();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                var value = ReadString(length);

                if (value != "")
                {
                    strings.Add(value);
                }

                return value;
            }

            return strings[reference >> 1];
        }

        public XmlDocument ReadAmf3XmlDocument()
        {
            int reference = ReadAmf3UInt29();

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
            int reference = ReadAmf3UInt29();

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
            int reference = ReadAmf3UInt29();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                var value = new Amf3Array()
                {
                    StrictDense = new List<object>(),
                    
                    SparseAssociative = new Dictionary<string, object>()
                };

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
            int reference = ReadAmf3UInt29();

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

                    reference = reference >> 1;

                    int length = reference;

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
                    var externizable = (IExternalizable)value.ToObject();

                        externizable.Read(this);
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

        public byte[] ReadAmf3ByteArray()
        {
            int reference = ReadAmf3UInt29();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                var value = new byte[length];

                objects.Add(value);
                
                for (int i = 0; i < length; i++)
                {
                    byte data = ReadByte();

                    value[i] = data;
                }

                return value;
            }

            return ( byte[] )objects[reference >> 1];
        }

        public List<int> ReadAmf3Int32List()
        {
            int reference = ReadAmf3UInt29();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                bool fixedVector = ReadBoolean();

                var value = new List<int>();

                objects.Add(value);
                
                for (int i = 0; i < length; i++)
                {
                    int data = ReadInt32();

                    value.Add(data);
                }

                return value;
            }

            return ( List<int> )objects[reference >> 1];
        }

        public List<uint> ReadAmf3UInt32List()
        {
            int reference = ReadAmf3UInt29();

            if ( (reference & 0x01) == 0x01)
            {
                int length = reference >> 1;

                bool fixedVector = ReadBoolean();

                var value = new List<uint>();

                objects.Add(value);
                
                for (int i = 0; i < length; i++)
                {
                    uint data = ReadUInt32();

                    value.Add(data);
                }

                return value;
            }

            return ( List<uint> )objects[reference >> 1];
        }

        public List<double> ReadAmf3DoubleList()
        {
            int reference = ReadAmf3UInt29();

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

        public List<object> ReadAmf3ObjectList()
        {
            int reference = ReadAmf3UInt29();

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
            int reference = ReadAmf3UInt29();

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
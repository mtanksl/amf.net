using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace mtanksl.ActionMessageFormat
{
    public class AmfWriter
    {
        private List<byte> data = new List<byte>();

        public byte[] Data
        {
            get
            {
                return data.ToArray();
            }
        }

        private List<string> strings = new List<string>();

        private List<object> objects = new List<object>();

        private List<Amf3Trait> traits = new List<Amf3Trait>();

        public void WriteAmfPacket(AmfPacket value)
        {
            WriteInt16( (short)value.Version );

            WriteAmfHeaders(value.Version, value.Headers);

            WriteAmfMessages(value.Version, value.Messages);
        }

        public void WriteAmfHeaders(AmfVersion version, List<AmfHeader> value)
        {
            WriteInt16( (short)value.Count );

            for (int i = 0; i < value.Count; i++)
            {
                WriteAmf0String(value[i].Name);

                WriteBoolean(value[i].MustUnderstand);

                WriteInt32(-1);

                if (version == AmfVersion.Amf0)
                {
                    WriteAmf0(value[i].Data);
                }
                else
                {
                    WriteByte( (byte)Amf0Type.Amf3 );

                    WriteAmf3(value[i].Data);
                }                                
            }
        }

        public void WriteAmfMessages(AmfVersion version, List<AmfMessage> value)
        {
            WriteInt16( (short)value.Count );

            for (int i = 0; i < value.Count; i++)
            {
                WriteAmf0String(value[i].TargetUri);

                WriteAmf0String(value[i].ResponseUri);

                WriteInt32(-1);

                if (version == AmfVersion.Amf0)
                {
                    WriteAmf0(value[i].Data);
                }
                else
                {
                    WriteByte( (byte)Amf0Type.Amf3 );

                    WriteAmf3(value[i].Data);
                } 

                strings.Clear();

                objects.Clear();

                traits.Clear();
            }
        }

        public void WriteByte(byte value)
        {
            data.Add(value);
        }

        public void WriteBoolean(bool value)
        {
            data.AddRange( BitConverter.GetBytes(value) );
        }

        public void WriteInt16(short value)
        {
            data.AddRange( BitConverter.GetBytes(value).Reverse() );
        }

        public void WriteInt32(int value)
        {
            data.AddRange( BitConverter.GetBytes(value).Reverse() );
        }

        public void WriteDouble(double value)
        {
            data.AddRange( BitConverter.GetBytes(value).Reverse() );
        }

        public void WriteString(string value)
        {
            data.AddRange( Encoding.UTF8.GetBytes(value) );
        }

        public void WriteAmf0(object value)
        {
            if (value is short || value is int || value is long || value is decimal || value is double)
            {
                WriteByte( (byte)Amf0Type.Number );

                WriteDouble( (double)value );
            }
            else if (value is bool)
            {
                WriteByte( (byte)Amf0Type.Boolean);

                WriteBoolean( (bool)value );
            }
            else if (value is string)
            {
                var s = (string)value;

                if (s.Length > 65535)
                {
                    WriteByte( (byte)Amf0Type.LongString);

                    WriteAmf0LongString(s);
                }
                else
                {
                    WriteByte( (byte)Amf0Type.String);

                    WriteAmf0String(s);
                }
            }
            else if (value is Amf0Object)
            {
                var o = (Amf0Object)value;

                if ( objects.Contains(o) )
                {
                    WriteByte( (byte)Amf0Type.Reference);

                    WriteAmf0ObjectReference(o);
                }
                else
                {
                    if (o.IsAnonymous)
                    {
                        WriteByte( (byte)Amf0Type.Object);

                        WriteAmf0Object(o); 
                    }
                    else
                    {
                        WriteByte( (byte)Amf0Type.TypedObject);

                        WriteAmf0Object(o); 
                    }                    
                }
            }
            else if (value is Dictionary<string, object>)
            {
                WriteByte( (byte)Amf0Type.EcmaArray);

                WriteAmf0Array( (Dictionary<string, object>)value );
            }
            else if (value is List<object>)
            {
                WriteByte( (byte)Amf0Type.StrictArray);

                WriteAmf0StrictArray( (List<object>)value );
            }
            else if (value is DateTime)
            {
                WriteByte( (byte)Amf0Type.Date);

                WriteAmf0Date( (DateTime)value );
            }
            else if (value is XmlDocument)
            {
                WriteByte( (byte)Amf0Type.XMLDocument);

                WriteAmf0XmlDocument( (XmlDocument)value );
            }
            else
            {
                WriteByte( (byte)Amf0Type.Null);
            }
        }

        public void WriteAmf0String(string value)
        {
            WriteInt16( (short)value.Length );

            WriteString(value);
        }

        public void WriteAmf0Object(Amf0Object value)
        {
            objects.Add(value);

            foreach (var item in value.DynamicMembersAndValues)
            {
                WriteAmf0String(item.Key);

                WriteAmf0(item.Value);
            }

            WriteAmf0String("");

            WriteByte( (byte)Amf0Type.ObjectEnd);
        }

        public void WriteAmf0ObjectReference(Amf0Object value)
        {
            WriteInt32( objects.IndexOf(value) );
        }

        public void WriteAmf0Array(Dictionary<string, object> value)
        {
            WriteInt32(value.Count);

            foreach (var item in value)
            {
                WriteAmf0String(item.Key);

                WriteAmf0(item.Value);
            }
        }

        public void WriteAmf0StrictArray(List<object> value)
        {
            WriteInt32(value.Count);

            foreach (var item in value)
            {
                WriteAmf0(item);
            }
        }

        public void WriteAmf0Date(DateTime value)
        {
            WriteDouble(value.Subtract( new DateTime(1970, 1, 1, 0, 0, 0) ).TotalMilliseconds);

            WriteInt16(-1);
        }

        public void WriteAmf0LongString(string value)
        {
            WriteInt32( (short)value.Length );

            WriteString(value);
        }

        public void WriteAmf0XmlDocument(XmlDocument value)
        {
            WriteAmf0LongString(value.OuterXml);
        }

        public void WriteAmf0TypedObject(Amf0Object value)
        {
            objects.Add(value);

            WriteAmf0String(value.ClassName);

            foreach (var item in value.DynamicMembersAndValues)
            {
                WriteAmf0String(item.Key);

                WriteAmf0(item.Value);
            }

            WriteAmf0String("");

            WriteByte( (byte)Amf0Type.ObjectEnd);
        }

        public void WriteAmf3(object value)
        {
            if (value is bool)
            {
                var b = (bool)value;

                if (b)
                {
                    WriteByte( (byte)Amf3Type.BooleanTrue);
                }
                else
                {
                    WriteByte( (byte)Amf3Type.BooleanFalse);
                }
            }
            else if (value is short || value is int)
            {
                WriteByte( (byte)Amf3Type.Integer);

                WriteAmf3Int32( (int)value);
            }
            else if (value is long || value is decimal || value is double)
            {
                WriteByte( (byte)Amf3Type.Double);

                WriteDouble( (double)value );
            }
            else if (value is string)
            {
                WriteByte( (byte)Amf3Type.String);

                WriteAmf3String( (string)value );
            }
            else if (value is XmlDocument)
            {
                WriteByte( (byte)Amf3Type.XmlDocument);

                WriteAmf3XmlDocument( (XmlDocument)value );   
            }
            else if (value is DateTime)
            {
                WriteByte( (byte)Amf3Type.Date);

                WriteAmf3Date( (DateTime)value );   
            }
            else if (value is Amf3Array)
            {
                WriteByte( (byte)Amf3Type.Array);

                WriteAmf3Array( (Amf3Array)value );   
            }
            else if (value is Amf3Object)
            {
                WriteByte( (byte)Amf3Type.Object);

                WriteAmf3Object( (Amf3Object)value );   
            }
            else if (value is List<byte>)
            {
                WriteByte( (byte)Amf3Type.ByteArray);

                WriteAmf3ByteArray( (List<byte>)value );   
            }
            else if (value is List<int>)
            {
                WriteByte( (byte)Amf3Type.VectorInt);

                WriteAmf3Int32Array( (List<int>)value );   
            }
            else if (value is List<double>)
            {
                WriteByte( (byte)Amf3Type.VectorDouble);

                WriteAmf3DoubleArray( (List<double>)value );   
            }
            else if (value is List<object>)
            {
                WriteByte( (byte)Amf3Type.VectorObject);

                WriteAmf3ObjectArray( (List<object>)value );   
            }
            else if (value is Dictionary<object, object>)
            {
                WriteByte( (byte)Amf3Type.Dictionary);

                WriteAmf3Dictionary( (Dictionary<object, object>)value );   
            }
            else
            {
                if (value != null)
                {
                    var data = new Amf3Object();

                    data.FromObject(value);

                    WriteAmf3(data);
                }
                else
                {
                    WriteByte( (byte)Amf3Type.Null);
                }
            }
        }

        public void WriteAmf3Int32(int value)
        {
            if (value < 128)
            {
                WriteByte( (byte)( (value & 0b00000000_00000000_00000000_01111111) >> 0  | 0b00000000_00000000_00000000_00000000) );
            }
            else if (value < 16384)
            {
                WriteByte( (byte)( (value & 0b00000000_00000000_00111111_10000000) >> 7  | 0b00000000_00000000_00000000_10000000) );

                WriteByte( (byte)( (value & 0b00000000_00000000_00000000_01111111) >> 0  | 0b00000000_00000000_00000000_00000000) );
            }
            else if (value < 2097152)
            {
                WriteByte( (byte)( (value & 0b00000000_00011111_11000000_00000000) >> 14 | 0b00000000_00000000_00000000_10000000) );

                WriteByte( (byte)( (value & 0b00000000_00000000_00111111_10000000) >> 7  | 0b00000000_00000000_00000000_10000000) );

                WriteByte( (byte)( (value & 0b00000000_00000000_00000000_01111111) >> 0  | 0b00000000_00000000_00000000_00000000) );
            }
            else
            {
                WriteByte( (byte)( (value & 0b00011111_11100000_00000000_00000000) >> 21 | 0b00000000_00000000_00000000_10000000) );

                WriteByte( (byte)( (value & 0b00000000_00011111_11000000_00000000) >> 14 | 0b00000000_00000000_00000000_10000000) );

                WriteByte( (byte)( (value & 0b00000000_00000000_00111111_10000000) >> 7  | 0b00000000_00000000_00000000_10000000) );

                WriteByte( (byte)( (value & 0b00000000_00000000_00000000_01111111) >> 0  | 0b00000000_00000000_00000000_00000000) );
            }
        }

        public void WriteAmf3String(string value)
        {
            if ( !strings.Contains(value) )
            {
                strings.Add(value);

                WriteAmf3Int32(value.Length << 1 | 1);

                WriteString(value);
            }
            else
            {
                WriteAmf3Int32(strings.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3XmlDocument(XmlDocument value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.OuterXml.Length << 1 | 1);

                WriteString(value.OuterXml);
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3Date(DateTime value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(1);

                WriteDouble(value.Subtract( new DateTime(1970, 1, 1, 0, 0, 0) ).TotalMilliseconds);
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3Array(Amf3Array value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.StrictDense.Count << 1 | 1);

                foreach (var item in value.SparseAssociative)
                {
                    WriteAmf3String(item.Key);

                    WriteAmf3(item.Value);
                }

                WriteAmf3String("");

                foreach (var item in value.StrictDense)
                {
                    WriteAmf3(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3Object(Amf3Object value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                if ( !traits.Contains(value.Trait) )
                {
                    traits.Add(value.Trait);

                    WriteAmf3Int32(value.Trait.Members.Count << 4 | (value.Trait.IsDynamic ? 8 : 0) | (value.Trait.IsExternalizable ? 4 : 0) | 2 | 1);

                    WriteAmf3String(value.Trait.ClassName);

                    foreach (var item in value.Trait.Members)
                    {
                        WriteAmf3String(item);
                    }
                }
                else
                {
                    WriteAmf3Int32(traits.IndexOf(value.Trait) << 2 | 0 | 1);
                }

                if (value.Trait.IsExternalizable)
                {
                    if (value.Trait.ClassName == "flex.messaging.io.ArrayCollection" || value.Trait.ClassName == "flex.messaging.io.ObjectProxy")
                    {
                        WriteAmf3( value.Values[0] );
                    }
                    else
                    {
                        ( (IExternalizable)value.ToObject() ).Write(this);
                    }
                }
                else
                {
                    foreach (var item in value.Values)
                    {
                        WriteAmf3(item);
                    }

                    if (value.Trait.IsDynamic)
                    {
                        foreach (var item in value.DynamicMembersAndValues)
                        {
                            WriteAmf3String(item.Key);

                            WriteAmf3(item.Value);
                        }

                        WriteAmf3String("");
                    }
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3ByteArray(List<byte> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 1);

                foreach (var item in value)
                {
                    WriteByte(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3Int32Array(List<int> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 1);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteAmf3Int32(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3DoubleArray(List<double> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 1);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteDouble(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3ObjectArray(List<object> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 1);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteAmf3(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }

        public void WriteAmf3Dictionary(Dictionary<object, object> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 1);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteAmf3(item.Key);

                    WriteAmf3(item.Value);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0);
            }
        }
    }
}
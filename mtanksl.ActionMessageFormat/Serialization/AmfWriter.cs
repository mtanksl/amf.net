using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace mtanksl.ActionMessageFormat
{
    public class AmfWriter
    {
        public const int MaxAmf0StringLength = 65535;

        public const int MaxAmf3Int32Value = 536870911;

        public const int MinAmf3Int32Value = 0;


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

        private Dictionary<object, Amf0Object> amf0References = new Dictionary<object, Amf0Object>();

        private Dictionary<object, Amf3Object> amf3References = new Dictionary<object, Amf3Object>();

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

        public void WriteUInt16(ushort value)
        {
            data.AddRange( BitConverter.GetBytes(value).Reverse() );
        }

        public void WriteInt32(int value)
        {
            data.AddRange( BitConverter.GetBytes(value).Reverse() );
        }

        public void WriteUInt32(uint value)
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
                
                strings.Clear();

                objects.Clear();

                traits.Clear();
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
        
        public void WriteAmf0(object value)
        {
            if (value == null)
            {
                WriteByte( (byte)Amf0Type.Null );
            }
            else if (value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is decimal || value is double)
            {
                WriteByte( (byte)Amf0Type.Number );

                WriteDouble( Convert.ToDouble(value) );
            }
            else if (value is bool)
            {
                WriteByte( (byte)Amf0Type.Boolean );

                WriteBoolean( (bool)value );
            }
            else if (value is string s)
            {
                if (s.Length > MaxAmf0StringLength)
                {
                    WriteByte( (byte)Amf0Type.LongString );

                    WriteAmf0LongString(s);
                }
                else
                {
                    WriteByte( (byte)Amf0Type.String );

                    WriteAmf0String(s);
                }
            }
            else if (value is Amf0Object o)
            {
                if ( objects.Contains(o) )
                {
                    WriteByte( (byte)Amf0Type.Reference );

                    WriteAmf0ObjectReference(o);
                }
                else
                {
                    if (o.IsAnonymous)
                    {
                        WriteByte( (byte)Amf0Type.Object );

                        WriteAmf0Object(o); 
                    }
                    else
                    {
                        WriteByte( (byte)Amf0Type.TypedObject );

                        WriteAmf0TypedObject(o); 
                    }
                }
            }
            else if (value is Dictionary<string, object>)
            {
                WriteByte( (byte)Amf0Type.EcmaArray );

                WriteAmf0Array( (Dictionary<string, object>)value );
            }
            else if (value is List<object>)
            {
                WriteByte( (byte)Amf0Type.StrictArray );

                WriteAmf0StrictArray( (List<object>)value );
            }
            else if (value is DateTime)
            {
                WriteByte( (byte)Amf0Type.Date );

                WriteAmf0Date( (DateTime)value );
            }
            else if (value is XmlDocument)
            {
                WriteByte( (byte)Amf0Type.XMLDocument );

                WriteAmf0XmlDocument( (XmlDocument)value );
            }
            else
            {
                Amf0Object amf0Rererence;

                if ( !amf0References.TryGetValue(value, out amf0Rererence) )
                {
                    amf0Rererence = new Amf0Object() 
                    {
                        ClassName = "", 
                        
                        DynamicMembersAndValues = new Dictionary<string, object>()
                    };

                    amf0Rererence.Read(value);
                    
                    amf0References.Add(value, amf0Rererence);
                }

                WriteAmf0(amf0Rererence);
            }
        }

        public void WriteAmf0String(string value)
        {
            WriteInt16( (short)value.Length );

            WriteString(value);
        }
        
        public void WriteAmf0LongString(string value)
        {
            WriteInt32( (int)value.Length );

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

            WriteInt16(0);
        }

        public void WriteAmf0XmlDocument(XmlDocument value)
        {
            WriteAmf0LongString(value.OuterXml);
        }

        public void WriteAmf3(object value)
        {
            if (value == null)
            {
                WriteByte( (byte)Amf3Type.Null );
            }
            else if (value is bool b)
            {
                if (b)
                {
                    WriteByte( (byte)Amf3Type.BooleanTrue );
                }
                else
                {
                    WriteByte( (byte)Amf3Type.BooleanFalse );
                }
            }
            else if (value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is decimal || value is double)
            {
                double i = Convert.ToDouble(value);

                if (i < MinAmf3Int32Value || i > MaxAmf3Int32Value)
                {
                    WriteByte( (byte)Amf3Type.Double);

                    WriteDouble(i);
                }
                else
                {
                    WriteByte( (byte)Amf3Type.Integer);

                    WriteAmf3Int32( Convert.ToInt32(value) );
                }
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
            else if (value is byte[])
            {
                WriteByte( (byte)Amf3Type.ByteArray);

                WriteAmf3ByteArray( (byte[])value );   
            }
            else if (value is List<int>)
            {
                WriteByte( (byte)Amf3Type.VectorInt);

                WriteAmf3Int32List( (List<int>)value );   
            }
            else if (value is List<uint>)
            {
                WriteByte( (byte)Amf3Type.VectorUInt);

                WriteAmf3UInt32List( (List<uint>)value );   
            }
            else if (value is List<double>)
            {
                WriteByte( (byte)Amf3Type.VectorDouble);

                WriteAmf3DoubleList( (List<double>)value );   
            }
            else if (value is List<object>)
            {
                WriteByte( (byte)Amf3Type.VectorObject);

                WriteAmf3ObjectList( (List<object>)value );   
            }
            else if (value is Dictionary<object, object>)
            {
                WriteByte( (byte)Amf3Type.Dictionary);

                WriteAmf3Dictionary( (Dictionary<object, object>)value );   
            }
            else
            {
                Amf3Object amf3Rererence;

                if ( !amf3References.TryGetValue(value, out amf3Rererence) )
                {
                    amf3Rererence = new Amf3Object() 
                    {
                        Trait = new Amf3Trait() 
                        { 
                            ClassName = "", 
                            
                            IsDynamic = false, 
                            
                            IsExternalizable = false,
                            
                            Members = new List<string>()                        
                        }, 
                        
                        Values = new List<object>(), 
                        
                        DynamicMembersAndValues = new Dictionary<string, object>() 
                    };

                    amf3Rererence.Read(value);

                    amf3References.Add(value, amf3Rererence);
                }

                WriteAmf3(amf3Rererence);
            }
        }

        public void WriteAmf3Int32(int value)
        {
            if (value >= 0x00 && value <= 0x7F)
            {

                WriteByte( (byte)( ( ( ( 0x7F << 0 ) & value ) >> 0 ) | 0x00 ) );

            }
            else if (value >= 0x80 && value <= 0x3FFF)
            {

                WriteByte( (byte)( ( ( ( 0x7F << 7 ) & value ) >> 7 ) | 0x80 ) );

                WriteByte( (byte)( ( ( ( 0x7F << 0 ) & value ) >> 0 ) | 0x00 ) );

            }
            else if (value >= 0x4000 && value <= 0x1FFFFF)
            {

                WriteByte( (byte)( ( ( ( 0x7F << 14 ) & value ) >> 14 ) | 0x80 ) );

                WriteByte( (byte)( ( ( ( 0x7F << 7 ) & value ) >> 7 ) | 0x80 ) );

                WriteByte( (byte)( ( ( ( 0x7F << 0 ) & value ) >> 0 ) | 0x00 ) );

            }
            else  if (value >= 0x200000 && value <= 0x3FFFFFFF)
            {

                WriteByte( (byte)( ( ( ( 0x7F << 22 ) & value ) >> 22 ) | 0x80 ) );

                WriteByte( (byte)( ( ( ( 0x7F << 15 ) & value ) >> 15 ) | 0x80 ) );

                WriteByte( (byte)( ( ( ( 0x7F << 8 ) & value ) >> 8 ) | 0x80 ) );

                WriteByte( (byte)( ( ( ( 0xFF << 0 ) & value ) >> 0 ) | 0x00 ) );

            }
        }

        public void WriteAmf3String(string value)
        {
            if ( !strings.Contains(value) )
            {
                strings.Add(value);

                WriteAmf3Int32(value.Length << 1 | 0x01);

                WriteString(value);
            }
            else
            {
                WriteAmf3Int32(strings.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3XmlDocument(XmlDocument value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.OuterXml.Length << 1 | 0x01);

                WriteString(value.OuterXml);
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3Date(DateTime value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(0x01);

                WriteDouble(value.Subtract( new DateTime(1970, 1, 1, 0, 0, 0) ).TotalMilliseconds);
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3Array(Amf3Array value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.StrictDense.Count << 1 | 0x01);

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
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
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

                    WriteAmf3Int32(value.Trait.Members.Count << 4 | (value.Trait.IsDynamic ? 0x01 : 0x00) << 3 | (value.Trait.IsExternalizable ? 0x01 : 0x00) << 2 | 0x01 << 1 | 0x01);

                    WriteAmf3String(value.Trait.ClassName);

                    foreach (var item in value.Trait.Members)
                    {
                        WriteAmf3String(item);
                    }
                }
                else
                {
                    WriteAmf3Int32(traits.IndexOf(value.Trait) << 2 | 0x00 << 1 | 0x01);
                }

                if (value.Trait.IsExternalizable)
                {
                    ( (IExternalizable)value.ToObject() ).Write(this);
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
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3ByteArray(byte[] value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Length << 1 | 0x01);

                foreach (var item in value)
                {
                    WriteByte(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3Int32List(List<int> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 0x01);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteInt32(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3UInt32List(List<uint> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 0x01);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteUInt32(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3DoubleList(List<double> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 0x01);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteDouble(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3ObjectList(List<object> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 0x01);

                WriteBoolean(false);

                WriteAmf3String("*");
                
                foreach (var item in value)
                {
                    WriteAmf3(item);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }

        public void WriteAmf3Dictionary(Dictionary<object, object> value)
        {
            if ( !objects.Contains(value) )
            {
                objects.Add(value);

                WriteAmf3Int32(value.Count << 1 | 0x01);

                WriteBoolean(false);

                foreach (var item in value)
                {
                    WriteAmf3(item.Key);

                    WriteAmf3(item.Value);
                }
            }
            else
            {
                WriteAmf3Int32(objects.IndexOf(value) << 1 | 0x00);
            }
        }
    }
}
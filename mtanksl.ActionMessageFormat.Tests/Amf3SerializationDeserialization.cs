using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class Amf3SerializationDeserialization
    {
        [TestMethod]
        public void TestAmf3Packet()
        {
            var writer = new AmfWriter();

                writer.WriteAmfPacket(new AmfPacket() 
                { 
                    Version = AmfVersion.Amf3,

                    Headers = new List<AmfHeader>()
                    {
                        new AmfHeader()
                        {
                            Name = "",

                            MustUnderstand = false,

                            Data = null
                        }
                    },

                    Messages = new List<AmfMessage>()
                    {
                        new AmfMessage()
                        {
                            TargetUri = "",

                            ResponseUri = "",

                            Data = null
                        }
                    }
                } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmfPacket();

            Assert.AreEqual("", data.Messages[0].TargetUri);
        }

        [Ignore]
        [TestMethod]
        public void TestAmf3Int32()
        {
            for (int i = 0; i < Math.Pow(2, 29) - 1; i++)
            {
                var writer = new AmfWriter();

                    writer.WriteAmf3Int32(i);

                var reader = new AmfReader(writer.Data);

                Assert.AreEqual(i, reader.ReadAmf3Int32() );
            }            
        }

        [TestMethod]
        public void TestAmf3String()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3String("Hello World");

            var reader = new AmfReader(writer.Data);

            Assert.AreEqual("Hello World", reader.ReadAmf3String() );
        }

        [TestMethod]
        public void TestAmf3Date()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3Date(new DateTime(2020, 12, 31, 23, 59, 59) );

            var reader = new AmfReader(writer.Data);

            Assert.AreEqual(new DateTime(2020, 12, 31, 23, 59, 59), reader.ReadAmf3Date() );
        }
        
        [TestMethod]
        public void TestAmf3Array()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3Array(new Amf3Array()
                { 
                    StrictDense = new List<object>() 
                    { 
                        byte.MaxValue,                        
                        false,                         
                        true,                         
                        short.MaxValue,                         
                        int.MaxValue,                         
                        double.MaxValue,                        
                        "Hello World" 
                    }, 
                    SparseAssociative = new Dictionary<string, object>() 
                    {
                        { "byte", byte.MaxValue },                        
                        { "false", false },                         
                        { "true", true },                         
                        { "short", short.MaxValue },                         
                        { "int", int.MaxValue },                        
                        { "double", double.MaxValue },                        
                        { "string", "Hello World" } 
                    } 
                } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3Array();

            // Warning: Numbers are serialized and deserialized as int or double
            
            Assert.AreEqual(byte.MaxValue, (int)data.StrictDense[0] );

            Assert.AreEqual(false, (bool)data.StrictDense[1] );

            Assert.AreEqual(true, (bool)data.StrictDense[2] );

            Assert.AreEqual(short.MaxValue, (int)data.StrictDense[3] );

            Assert.AreEqual(int.MaxValue, (double)data.StrictDense[4] );

            Assert.AreEqual(double.MaxValue, (double)data.StrictDense[5] );

            Assert.AreEqual("Hello World", (string)data.StrictDense[6] );


            Assert.AreEqual(byte.MaxValue, (int)data.SparseAssociative["byte"] );

            Assert.AreEqual(false, (bool)data.SparseAssociative["false"] );

            Assert.AreEqual(true, (bool)data.SparseAssociative["true"] );

            Assert.AreEqual(short.MaxValue, (int)data.SparseAssociative["short"] );

            Assert.AreEqual(int.MaxValue, (double)data.SparseAssociative["int"] );

            Assert.AreEqual(double.MaxValue, (double)data.SparseAssociative["double"] );

            Assert.AreEqual("Hello World", (string)data.SparseAssociative["string"] );
        }

        [TestMethod]
        public void TestAmf3DynamicObject()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3Object(new Amf3Object() 
                {
                    Trait = new Amf3Trait() 
                    {
                        ClassName = "", 
                        
                        IsDynamic = true, 
                        
                        IsExternalizable = false,
                        
                        Members = new List<string>()                     
                    },

                    Values = new List<object>(), 
                    
                    DynamicMembersAndValues = new Dictionary<string, object>()
                    {
                        { "byte", byte.MaxValue },                         
                        { "false", false },                         
                        { "true", true },                        
                        { "short", short.MaxValue },                         
                        { "int", int.MaxValue },                         
                        { "double", double.MaxValue },                         
                        { "string", "Hello World" } 
                    } 
                } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3Object();

            // Warning: Numbers are serialized and deserialized as int or double

            Assert.AreEqual("", data.Trait.ClassName);

            Assert.AreEqual(byte.MaxValue, (int)data.DynamicMembersAndValues["byte"] );

            Assert.AreEqual(false, (bool)data.DynamicMembersAndValues["false"] );

            Assert.AreEqual(true, (bool)data.DynamicMembersAndValues["true"] );

            Assert.AreEqual(short.MaxValue, (int)data.DynamicMembersAndValues["short"] );

            Assert.AreEqual(int.MaxValue, (double)data.DynamicMembersAndValues["int"] );

            Assert.AreEqual(double.MaxValue, (double)data.DynamicMembersAndValues["double"] );

            Assert.AreEqual("Hello World", (string)data.DynamicMembersAndValues["string"] );
        }

        [TestMethod]
        public void TestAmf3ExternizableObject()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3Object(new Amf3Object() 
                {
                    Trait = new Amf3Trait() 
                    {
                        ClassName = "DSC", 
                        
                        IsDynamic = false, 
                        
                        IsExternalizable = true, 
                        
                        Members = new List<string>()
                        {
                            "operation",
                            "correlationId",
                            "correlationIdBytes",
                            "body",
                            "clientId",
                            "clientIdBytes",
                            "destination",
                            "headers",
                            "messageId",
                            "messageIdBytes",
                            "timestamp",
                            "timeToLive"
                        }
                    }, 

                    Values = new List<object>()
                    {
                        0,
                        "",
                        null,
                        null,
                        "",
                        null,
                        "",
                        null,
                        "",
                        null,
                        0.0,
                        0.0
                    },
                    
                    DynamicMembersAndValues = new Dictionary<string, object>()
                } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3Object();

            // Warning: Numbers are serialized and deserialized as int or double

            Assert.AreEqual("Hello World", data.Trait.ClassName);
        }

        [TestMethod]
        public void TestAmf3Object()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3Object(new Amf3Object() 
                {
                    Trait = new Amf3Trait() 
                    {
                        ClassName = "Hello World", 
                        
                        IsDynamic = false,
                        
                        IsExternalizable = false, 
                        
                        Members = new List<string>()
                        {
                            "byte",                            
                            "false",                             
                            "true",                                     
                            "short",                             
                            "int",                            
                            "double",                            
                            "string"                         
                        } 
                    }, 
                    
                    Values = new List<object>() 
                    {
                        byte.MaxValue,                         
                        false,                         
                        true,                         
                        short.MaxValue,                        
                        int.MaxValue,                         
                        double.MaxValue,                         
                        "Hello World"
                    }, 
                    
                    DynamicMembersAndValues = new Dictionary<string, object>()
                } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3Object();

            // Warning: Numbers are serialized and deserialized as int or double

            Assert.AreEqual("Hello World", data.Trait.ClassName);

            Assert.AreEqual(byte.MaxValue, (int)data.Values[ data.Trait.Members.IndexOf("byte") ] );

            Assert.AreEqual(false, (bool)data.Values[ data.Trait.Members.IndexOf("false") ] );

            Assert.AreEqual(true, (bool)data.Values[ data.Trait.Members.IndexOf("true") ] );

            Assert.AreEqual(short.MaxValue, (int)data.Values[ data.Trait.Members.IndexOf("short") ] );

            Assert.AreEqual(int.MaxValue, (double)data.Values[ data.Trait.Members.IndexOf("int") ] );

            Assert.AreEqual(double.MaxValue, (double)data.Values[ data.Trait.Members.IndexOf("double") ] );

            Assert.AreEqual("Hello World", (string)data.Values[ data.Trait.Members.IndexOf("string") ] );
        }

        [TestMethod]
        public void TestAmf3ByteArray()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3ByteArray(new byte[] { byte.MaxValue } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3ByteArray();
            
            Assert.AreEqual(byte.MaxValue, data[0] );
        }

        [TestMethod]
        public void TestAmf3Int32List()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3Int32List(new List<int>() { int.MaxValue } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3Int32List();

            Assert.AreEqual(int.MaxValue, data[0] );
        }

        [TestMethod]
        public void TestAmf3UInt32List()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3UInt32List(new List<uint>() { uint.MaxValue } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3UInt32List();

            Assert.AreEqual(uint.MaxValue, data[0] );
        }

        [TestMethod]
        public void TestAmf3DoubleList()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3DoubleList(new List<double>() { double.MaxValue } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3DoubleList();

            Assert.AreEqual(double.MaxValue, data[0] );
        }

        [TestMethod]
        public void TestAmf3ObjectList()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3ObjectList(new List<object>()
                {
                    byte.MaxValue, 
                    false, 
                    true,
                    short.MaxValue, 
                    int.MaxValue,
                    double.MaxValue, 
                    "Hello World"
                } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3ObjectList();

            // Warning: Numbers are serialized and deserialized as int or double

            Assert.AreEqual(byte.MaxValue, (int)data[0] );

            Assert.AreEqual(false, (bool)data[1] );

            Assert.AreEqual(true, (bool)data[2] );

            Assert.AreEqual(short.MaxValue, (int)data[3] );

            Assert.AreEqual(int.MaxValue, (double)data[4] );

            Assert.AreEqual(double.MaxValue, (double)data[5] );

            Assert.AreEqual("Hello World", (string)data[6] );
        }

        [TestMethod]
        public void TestAmf3Dictionary()
        {
            var writer = new AmfWriter();

                writer.WriteAmf3Dictionary(new Dictionary<object, object>() 
                {
                    { "byte", byte.MaxValue },                     
                    { "false", false },                    
                    { "true", true },
                    { "short", short.MaxValue },                    
                    { "int", int.MaxValue },                     
                    { "double", double.MaxValue },                    
                    { "string", "Hello World" }
                } );

            var reader = new AmfReader(writer.Data);

                var data = reader.ReadAmf3Dictionary();

            // Warning: Numbers are serialized and deserialized as int or double

            Assert.AreEqual(byte.MaxValue, (int)data["byte"] );

            Assert.AreEqual(false, (bool)data["false"] );

            Assert.AreEqual(true, (bool)data["true"] );

            Assert.AreEqual(short.MaxValue, (int)data["short"] );

            Assert.AreEqual(int.MaxValue, (double)data["int"] );

            Assert.AreEqual(double.MaxValue, (double)data["double"] );

            Assert.AreEqual("Hello World", (string)data["string"] );
        }
    }
}
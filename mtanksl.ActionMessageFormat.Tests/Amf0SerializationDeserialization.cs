using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class Amf0SerializationDeserialization
    {
        [TestMethod]
        public void TestAmf0()
        {
            var value = new Test() 
            {
                Byte = byte.MaxValue,

                False = false,

                True = true,

                Short = short.MaxValue,

                Int = int.MaxValue,

                Double = double.MaxValue,

                String = "Hello World"
            };

            value.Reference = value;

            var writer = new AmfWriter();

                writer.WriteAmf0(value);

            var reader = new AmfReader(writer.Data);

                var data = (Test)( (Amf3Object)reader.ReadAmf0() ).ToObject;

            // Warning: Amf0Object are deserialized as Amf3Object

            Assert.AreEqual(byte.MaxValue, data.Byte);

            Assert.AreEqual(false, data.False);

            Assert.AreEqual(true, data.True);

            Assert.AreEqual(short.MaxValue, data.Short);

            Assert.AreEqual(int.MaxValue, data.Int);

            Assert.AreEqual(double.MaxValue, data.Double);

            Assert.AreEqual("Hello World", data.String);

            Assert.AreEqual(data, data.Reference);
        }

        [TestMethod]
        public void TestAmf0Packet()
        {
            var writer = new AmfWriter();

                writer.WriteAmfPacket(new AmfPacket() 
                { 
                    Version = AmfVersion.Amf0,

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

        [TestMethod]
        public void TestAmf0String()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0String("Hello World");

            var reader = new AmfReader(writer.Data);

            Assert.AreEqual("Hello World", reader.ReadAmf0String() );
        }

        [TestMethod]
        public void TestAmf0LongString()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0LongString("Hello World");

            var reader = new AmfReader(writer.Data);

            Assert.AreEqual("Hello World", reader.ReadAmf0LongString() );
        }

        [TestMethod]
        public void TestAmf0Object()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0Object(new Amf0Object() 
                { 
                    ClassName = "", 
                    
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

                var data = reader.ReadAmf0Object();

            // Warning: Numbers are serialized and deserialized as double

            Assert.AreEqual("", data.ClassName);

            Assert.AreEqual(byte.MaxValue, (double)data.DynamicMembersAndValues["byte"] );

            Assert.AreEqual(false, (bool)data.DynamicMembersAndValues["false"] );

            Assert.AreEqual(true, (bool)data.DynamicMembersAndValues["true"] );

            Assert.AreEqual(short.MaxValue, (double)data.DynamicMembersAndValues["short"] );

            Assert.AreEqual(int.MaxValue, (double)data.DynamicMembersAndValues["int"] );

            Assert.AreEqual(double.MaxValue, (double)data.DynamicMembersAndValues["double"] );

            Assert.AreEqual("Hello World", (string)data.DynamicMembersAndValues["string"] );
        }

        [TestMethod]
        public void TestAmf0TypedObject()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0TypedObject(new Amf0Object() 
                {
                    ClassName = "Hello World",
                    
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

                var data = reader.ReadAmf0TypedObject();

            // Warning: Numbers are serialized and deserialized as double

            Assert.AreEqual("Hello World", data.ClassName);

            Assert.AreEqual(byte.MaxValue, (double)data.DynamicMembersAndValues["byte"] );

            Assert.AreEqual(false, (bool)data.DynamicMembersAndValues["false"] );

            Assert.AreEqual(true, (bool)data.DynamicMembersAndValues["true"] );

            Assert.AreEqual(short.MaxValue, (double)data.DynamicMembersAndValues["short"] );

            Assert.AreEqual(int.MaxValue, (double)data.DynamicMembersAndValues["int"] );

            Assert.AreEqual(double.MaxValue, (double)data.DynamicMembersAndValues["double"] );

            Assert.AreEqual("Hello World", (string)data.DynamicMembersAndValues["string"] );
        }

        [TestMethod]
        public void TestAmf0Date()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0Date(new DateTime(2020, 12, 31, 23, 59, 59) );

            var reader = new AmfReader(writer.Data);

            Assert.AreEqual(new DateTime(2020, 12, 31, 23, 59, 59), reader.ReadAmf0Date() );
        }

        [TestMethod]
        public void TestAmf0Array()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0Array(new Dictionary<string, object>() 
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

                var data = reader.ReadAmf0Array();

            // Warning: Numbers are serialized and deserialized as double

            Assert.AreEqual(byte.MaxValue, (double)data["byte"] );

            Assert.AreEqual(false, (bool)data["false"] );

            Assert.AreEqual(true, (bool)data["true"] );

            Assert.AreEqual(short.MaxValue, (double)data["short"] );

            Assert.AreEqual(int.MaxValue, (double)data["int"] );

            Assert.AreEqual(double.MaxValue, (double)data["double"] );

            Assert.AreEqual("Hello World", (string)data["string"] );
        }

        [TestMethod]
        public void TestAmf0StrictArray()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0StrictArray(new List<object>()
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

                var data = reader.ReadAmf0StrictArray();

            // Warning: Numbers are serialized and deserialized as double

            Assert.AreEqual(byte.MaxValue, (double)data[0] );

            Assert.AreEqual(false, (bool)data[1] );

            Assert.AreEqual(true, (bool)data[2] );

            Assert.AreEqual(short.MaxValue, (double)data[3] );

            Assert.AreEqual(int.MaxValue, (double)data[4] );

            Assert.AreEqual(double.MaxValue, (double)data[5] );

            Assert.AreEqual("Hello World", (string)data[6] );
        }
    }
}
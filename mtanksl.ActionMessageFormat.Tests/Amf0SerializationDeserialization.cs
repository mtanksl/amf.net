using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class Amf0SerializationDeserialization
    {
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
        public void TestAmf0Array()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0Array(new Dictionary<string, object>() { { "byte", byte.MaxValue }, { "false", false }, { "true", true }, { "short", short.MaxValue }, { "int", int.MaxValue }, { "double", double.MaxValue }, { "string", "Hello World" } } );

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

                writer.WriteAmf0StrictArray(new List<object>() { byte.MaxValue, false, true, short.MaxValue, int.MaxValue, double.MaxValue, "Hello World" } );

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

        [TestMethod]
        public void TestWriteAmf0Date()
        {
            var writer = new AmfWriter();

                writer.WriteAmf0Date(new DateTime(2020, 12, 31, 23, 59, 59) );

            var reader = new AmfReader(writer.Data);

            Assert.AreEqual(new DateTime(2020, 12, 31, 23, 59, 59), reader.ReadAmf0Date() );
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class PrimitiveDeserialization
    {
        [TestMethod]
        public void TestByte()
        {
            var reader = new AmfReader( Convert.FromBase64String("/w==") );

            var data = reader.ReadByte();

            Assert.AreEqual(byte.MaxValue, data);
        }

        [TestMethod]
        public void TestFalse()
        {
            var reader = new AmfReader( Convert.FromBase64String("AA==") );

            var data = reader.ReadBoolean();

            Assert.AreEqual(false, data);
        }

        [TestMethod]
        public void TestTrue()
        {
            var reader = new AmfReader( Convert.FromBase64String("AQ==") );

            var data = reader.ReadBoolean();

            Assert.AreEqual(true, data);
        }

        [TestMethod]
        public void TestInt16()
        {
            var reader = new AmfReader( Convert.FromBase64String("f/8=") );

            var data = reader.ReadInt16();

            Assert.AreEqual(short.MaxValue, data);
        }

        [TestMethod]
        public void TestInt32()
        {
            var reader = new AmfReader( Convert.FromBase64String("f////w==") );

            var data = reader.ReadInt32();
            
            Assert.AreEqual(int.MaxValue, data);
        }

        [TestMethod]
        public void TestDouble()
        {
            var reader = new AmfReader( Convert.FromBase64String("f+////////8=") );

            var data = reader.ReadDouble();
            
            Assert.AreEqual(double.MaxValue, data);
        }

        [TestMethod]
        public void TestString()
        {
            var reader = new AmfReader( Convert.FromBase64String("SGVsbG8gV29ybGQ=") );

            var data = reader.ReadString(11);
            
            Assert.AreEqual("Hello World", data);
        }
    }
}
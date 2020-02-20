using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class PrimitiveSerialization
    {
        [TestMethod]
        public void TestByte()
        {
            var writer = new AmfWriter();

                writer.WriteByte(byte.MaxValue);

            var data = Convert.ToBase64String(writer.Data);

            Assert.AreEqual("/w==", data);
        }

        [TestMethod]
        public void TestFalse()
        {
            var writer = new AmfWriter();

                writer.WriteBoolean(false);

            var data = Convert.ToBase64String(writer.Data);

            Assert.AreEqual("AA==", data);
        }

        [TestMethod]
        public void TestTrue()
        {
            var writer = new AmfWriter();

                writer.WriteBoolean(true);

            var data = Convert.ToBase64String(writer.Data);

            Assert.AreEqual("AQ==", data);
        }

        [TestMethod]
        public void TestInt16()
        {
            var writer = new AmfWriter();

                writer.WriteInt16(short.MaxValue);

            var data = Convert.ToBase64String(writer.Data);

            Assert.AreEqual("f/8=", data);
        }

        [TestMethod]
        public void TestInt32()
        {
            var writer = new AmfWriter();

                writer.WriteInt32(int.MaxValue);

            var data = Convert.ToBase64String(writer.Data);

            Assert.AreEqual("f////w==", data);
        }

        [TestMethod]
        public void TestDouble()
        {
            var writer = new AmfWriter();

                writer.WriteDouble(double.MaxValue);

            var data = Convert.ToBase64String(writer.Data);

            Assert.AreEqual("f+////////8=", data);
        }

        [TestMethod]
        public void TestString()
        {
            var writer = new AmfWriter();

                writer.WriteString("Hello World");

            var data = Convert.ToBase64String(writer.Data);

            Assert.AreEqual("SGVsbG8gV29ybGQ=", data);
        }
    }
}
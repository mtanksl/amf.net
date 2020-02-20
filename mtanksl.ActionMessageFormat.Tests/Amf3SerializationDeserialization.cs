using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class Amf3SerializationDeserialization
    {
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
    }
}
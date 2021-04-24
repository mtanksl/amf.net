using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class ExternalizableSerializationDeserialization
    {
        [TestMethod]
        public void TestExternalizable()
        {
            var value = new ExternalizableTest() 
            {
                Value1 = 16180,

                Value2 = "Hello World"
            };
                       
            var writer = new AmfWriter();

                writer.WriteAmf3(value);

            var reader = new AmfReader(writer.Data);

                var data = (ExternalizableTest)( (Amf3Object)reader.ReadAmf3() ).ToObject();

            Assert.AreEqual(16180, data.Value1);

            Assert.AreEqual("Hello World", data.Value2);
        }

        [TestMethod]
        public void TestUnknownExternalizable()
        {
            var reader = new AmfReader( Convert.FromBase64String("CgczdW5rbm93bkV4dGVybmFsaXphYmxlVGVzdP8E/jQGF0hlbGxvIFdvcmxk") );

                var data = (UnknownExternalizableTest)( (Amf3Object)reader.ReadAmf3() ).ToObject();

            Console.WriteLine(data.UnknownBytes);
        }
    }
}
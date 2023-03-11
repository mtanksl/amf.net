using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class Examples
    {
        [TestMethod]
        public void Example1()
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

                            Data = new List<object>()
                            {
                                new Test() { String = "First" },

                                new { AString = "Second" },

                                new ExternalizableTest() { Value2 = "Third"},

                                "Fourth"
                            }
                        }
                    }
                } );

            var reader = new AmfReader(writer.Data);

                var packet = reader.ReadAmfPacket();

                var list = (List<object>)packet.Messages[0].Data;

                var test = ( (Amf3Object)list[0] ).ToObject<Test>();

                var anonymousObject = ( (Amf3Object)list[1] ).ToObject<dynamic>();

                var externizableObject = ( (Amf3Object)list[2] ).ToObject<ExternalizableTest>();

                var str = (string)list[3];

            Assert.AreEqual("First", test.String);

            Assert.AreEqual("Second", anonymousObject.AString);

            Assert.AreEqual("Third", externizableObject.Value2);

            Assert.AreEqual("Fourth", str);
        }
    }
}
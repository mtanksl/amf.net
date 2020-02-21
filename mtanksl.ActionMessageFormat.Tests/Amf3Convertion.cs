using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Dynamic;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class Amf3Convertion
    {
        [TestMethod]
        public void TestAmf3ObjectFromDynamic()
        {
            var obj = new Amf3Object() { Trait = new Amf3Trait() { ClassName = "", IsDynamic = false, IsExternalizable = false, Members = new List<string>() }, Values = new List<object>(), DynamicMembersAndValues = new Dictionary<string, object>() };

                dynamic value = new ExpandoObject();

                value.Byte = byte.MaxValue;

                value.False = false;

                value.True = true;

                value.Short = short.MaxValue;

                value.Int = int.MaxValue;

                value.Double = double.MaxValue;

                value.String = "Hello World";

            obj.Read(value);

            Assert.AreEqual(true, obj.Trait.IsAnonymous);

            Assert.AreEqual(true, obj.Trait.IsDynamic);

            Assert.AreEqual(false, obj.Trait.IsExternalizable);

            Assert.AreEqual(byte.MaxValue, obj.DynamicMembersAndValues["Byte"] );

            Assert.AreEqual(false, obj.DynamicMembersAndValues["False"] );

            Assert.AreEqual(true, obj.DynamicMembersAndValues["True"] );
            
            Assert.AreEqual(short.MaxValue, obj.DynamicMembersAndValues["Short"] );

            Assert.AreEqual(int.MaxValue, obj.DynamicMembersAndValues["Int"] );

            Assert.AreEqual(double.MaxValue, obj.DynamicMembersAndValues["Double"] );

            Assert.AreEqual("Hello World", obj.DynamicMembersAndValues["String"] );
        }

        [TestMethod]
        public void TestAmf3ObjectFromAnonymous()
        {
            var obj = new Amf3Object() { Trait = new Amf3Trait() { ClassName = "", IsDynamic = false, IsExternalizable = false, Members = new List<string>() }, Values = new List<object>(), DynamicMembersAndValues = new Dictionary<string, object>() };

                var value = new {

                    Byte = byte.MaxValue,

                    False = false,

                    True = true,

                    Short = short.MaxValue,

                    Int = int.MaxValue,

                    Double = double.MaxValue,

                    String = "Hello World"
                };

            obj.Read(value);

            Assert.AreEqual(true, obj.Trait.IsAnonymous);

            Assert.AreEqual(true, obj.Trait.IsDynamic);

            Assert.AreEqual(false, obj.Trait.IsExternalizable);

            Assert.AreEqual(byte.MaxValue, obj.DynamicMembersAndValues["Byte"] );

            Assert.AreEqual(false, obj.DynamicMembersAndValues["False"] );

            Assert.AreEqual(true, obj.DynamicMembersAndValues["True"] );
            
            Assert.AreEqual(short.MaxValue, obj.DynamicMembersAndValues["Short"] );

            Assert.AreEqual(int.MaxValue, obj.DynamicMembersAndValues["Int"] );

            Assert.AreEqual(double.MaxValue, obj.DynamicMembersAndValues["Double"] );

            Assert.AreEqual("Hello World", obj.DynamicMembersAndValues["String"] );
        }

        [TestMethod]
        public void TestAmf3ObjectFromClass()
        {
            var obj = new Amf3Object() { Trait = new Amf3Trait() { ClassName = "", IsDynamic = false, IsExternalizable = false, Members = new List<string>() }, Values = new List<object>(), DynamicMembersAndValues = new Dictionary<string, object>() };

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

            obj.Read(value);

            Assert.AreEqual(false, obj.Trait.IsAnonymous);

            Assert.AreEqual(false, obj.Trait.IsDynamic);

            Assert.AreEqual(false, obj.Trait.IsExternalizable);

            Assert.AreEqual(byte.MaxValue, obj.Values[ obj.Trait.Members.IndexOf("byte") ] );

            Assert.AreEqual(false, obj.Values[ obj.Trait.Members.IndexOf("false") ] );

            Assert.AreEqual(true, obj.Values[ obj.Trait.Members.IndexOf("true") ] );
            
            Assert.AreEqual(short.MaxValue, obj.Values[ obj.Trait.Members.IndexOf("short") ] );

            Assert.AreEqual(int.MaxValue, obj.Values[ obj.Trait.Members.IndexOf("int") ] );

            Assert.AreEqual(double.MaxValue, obj.Values[ obj.Trait.Members.IndexOf("double") ] );

            Assert.AreEqual("Hello World", obj.Values[ obj.Trait.Members.IndexOf("string") ] );
        }
    }
}
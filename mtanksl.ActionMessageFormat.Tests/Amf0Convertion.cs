using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Dynamic;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TestClass]
    public class Amf0Convertion
    {
        [TestMethod]
        public void TestAmf0ObjectFromDynamic()
        {
            var obj = new Amf0Object() { ClassName = "", DynamicMembersAndValues = new Dictionary<string, object>() };

                dynamic value = new ExpandoObject();

                value.Byte = byte.MaxValue;

                value.False = false;

                value.True = true;

                value.Short = short.MaxValue;

                value.Int = int.MaxValue;

                value.Double = double.MaxValue;

                value.String = "Hello World";

            obj.FromObject(value);

            Assert.AreEqual(true, obj.IsAnonymous);

            Assert.AreEqual(byte.MaxValue, obj.DynamicMembersAndValues["Byte"] );

            Assert.AreEqual(false, obj.DynamicMembersAndValues["False"] );

            Assert.AreEqual(true, obj.DynamicMembersAndValues["True"] );
            
            Assert.AreEqual(short.MaxValue, obj.DynamicMembersAndValues["Short"] );

            Assert.AreEqual(int.MaxValue, obj.DynamicMembersAndValues["Int"] );

            Assert.AreEqual(double.MaxValue, obj.DynamicMembersAndValues["Double"] );

            Assert.AreEqual("Hello World", obj.DynamicMembersAndValues["String"] );
        }

        [TestMethod]
        public void TestAmf0ObjectFromAnonymous()
        {
            var obj = new Amf0Object() { ClassName = "", DynamicMembersAndValues = new Dictionary<string, object>() };

                var value = new {

                    Byte = byte.MaxValue,

                    False = false,

                    True = true,

                    Short = short.MaxValue,

                    Int = int.MaxValue,

                    Double = double.MaxValue,

                    String = "Hello World"
                };

            obj.FromObject(value);

            Assert.AreEqual(true, obj.IsAnonymous);

            Assert.AreEqual(byte.MaxValue, obj.DynamicMembersAndValues["Byte"] );

            Assert.AreEqual(false, obj.DynamicMembersAndValues["False"] );

            Assert.AreEqual(true, obj.DynamicMembersAndValues["True"] );
            
            Assert.AreEqual(short.MaxValue, obj.DynamicMembersAndValues["Short"] );

            Assert.AreEqual(int.MaxValue, obj.DynamicMembersAndValues["Int"] );

            Assert.AreEqual(double.MaxValue, obj.DynamicMembersAndValues["Double"] );

            Assert.AreEqual("Hello World", obj.DynamicMembersAndValues["String"] );
        }

        [TestMethod]
        public void TestAmf0ObjectFromClass()
        {
            var obj = new Amf0Object() { ClassName = "", DynamicMembersAndValues = new Dictionary<string, object>() };

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

            obj.FromObject(value);

            Assert.AreEqual(false, obj.IsAnonymous);

            Assert.AreEqual(byte.MaxValue, obj.DynamicMembersAndValues["byte"] );

            Assert.AreEqual(false, obj.DynamicMembersAndValues["false"] );

            Assert.AreEqual(true, obj.DynamicMembersAndValues["true"] );
            
            Assert.AreEqual(short.MaxValue, obj.DynamicMembersAndValues["short"] );

            Assert.AreEqual(int.MaxValue, obj.DynamicMembersAndValues["int"] );

            Assert.AreEqual(double.MaxValue, obj.DynamicMembersAndValues["double"] );

            Assert.AreEqual("Hello World", obj.DynamicMembersAndValues["string"] );
        }        
    }
}
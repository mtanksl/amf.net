namespace mtanksl.ActionMessageFormat.Tests
{
    [TraitClass("externalizableTest")]
    public class ExternalizableTest : IExternalizable
    {
        [TraitMember("value1")]
        public int Value1 { get; set; }

        [TraitMember("value2")]
        public string Value2 { get; set; }

        public void Write(AmfWriter writer)
        {
            writer.WriteByte(0xFF);

            writer.WriteAmf3(Value1);

            writer.WriteAmf3(Value2);
        }

        public void Read(AmfReader reader)
        {
            byte anything = reader.ReadByte();

            Value1 = (int)reader.ReadAmf3(); // Warning: Numbers are serialized and deserialized as int or double

            Value2 = (string)reader.ReadAmf3();
        }
    }
}
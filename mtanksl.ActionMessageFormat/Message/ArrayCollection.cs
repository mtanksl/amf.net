namespace mtanksl.ActionMessageFormat
{
    [TraitClass("flex.messaging.io.ArrayCollection")]
    public class ArrayCollection : IExternalizable
    {
        [TraitMember("value")]
        public object Value { get; set; }
        
        public void Read(AmfReader reader)
        {
            Value = reader.ReadAmf3();
        }

        public void Write(AmfWriter writer)
        {
            writer.WriteAmf3(Value);
        }
    }
}
namespace mtanksl.ActionMessageFormat.Message
{
    [TraitClass("flex.messaging.io.ArrayCollection")]
    public class ArrayCollection : IExternalizable
    {
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
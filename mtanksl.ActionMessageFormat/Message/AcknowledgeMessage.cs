namespace mtanksl.ActionMessageFormat
{
    [TraitClass("DSK")]
    [TraitClass("flex.messaging.messages.AcknowledgeMessage")]
    [TraitClass("flex.messaging.messages.AcknowledgeMessageExt")]
    public class AcknowledgeMessage : AsyncMessage
    {
        public override void Read(AmfReader reader)
        {
            base.Read(reader);

            var flags = reader.ReadFlags();

            for (int i = 0; i < flags.Count; i++)
            {
                var flag = flags[i];
            }
        }

        public override void Write(AmfWriter writer)
        {
            base.Write(writer);

            writer.WriteByte(0);
        }
    }
}
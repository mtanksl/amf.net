namespace mtanksl.ActionMessageFormat
{
    [TraitClass("flex.messaging.messages.AcknowledgeMessage")]
    public class AcknowledgeMessage : AsyncMessage
    {
        public override IMessage SmallMessage()
        {
            return new AcknowledgeMessageExt(this);
        }

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
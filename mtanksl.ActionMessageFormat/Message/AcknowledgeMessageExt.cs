namespace mtanksl.ActionMessageFormat
{
    [TraitClass("flex.messaging.messages.AcknowledgeMessageExt")]
    [TraitClass("DSK")]
    public class AcknowledgeMessageExt : AcknowledgeMessage, IExternalizable
    {
        private AcknowledgeMessage acknowledgeMessage;

        public AcknowledgeMessageExt()
        {

        }

        public AcknowledgeMessageExt(AcknowledgeMessage acknowledgeMessage)
        {
            this.acknowledgeMessage = acknowledgeMessage;
        }

        public override void Read(AmfReader reader)
        {
            if (acknowledgeMessage != null)
            {
                acknowledgeMessage.Read(reader);
            }
            else
            {
                base.Read(reader);
            }
        }

        public override void Write(AmfWriter writer)
        {
            if (acknowledgeMessage != null)
            {
                acknowledgeMessage.Write(writer);
            }
            else
            {
                base.Write(writer);
            }
        }
    }
}
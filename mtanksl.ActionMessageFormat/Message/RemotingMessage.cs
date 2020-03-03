namespace mtanksl.ActionMessageFormat
{
    [TraitClass("flex.messaging.messages.RemotingMessage")]
    public class RemotingMessage : AbstractMessage
    {
        [TraitMember("operation")]
        public string Operation { get; set; }

        [TraitMember("source")]
        public string Source { get; set; }

        public override void Read(AmfReader reader)
        {
            base.Read(reader);


        }

        public override void Write(AmfWriter writer)
        {
            base.Write(writer);


        }
    }
}
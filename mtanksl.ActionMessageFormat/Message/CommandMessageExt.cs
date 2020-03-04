namespace mtanksl.ActionMessageFormat
{
    [TraitClass("flex.messaging.messages.CommandMessageExt")]
    [TraitClass("DSC")]
    public class CommandMessageExt : CommandMessage, IExternalizable
    {
        private CommandMessage commandMessage;

        public CommandMessageExt()
        {

        }

        public CommandMessageExt(CommandMessage commandMessage)
        {
            this.commandMessage = commandMessage;
        }

        public override void Read(AmfReader reader)
        {
            if (commandMessage != null)
            {
                commandMessage.Read(reader);
            }
            else
            {
                base.Read(reader);
            }
        }

        public override void Write(AmfWriter writer)
        {
            if (commandMessage != null)
            {
                commandMessage.Write(writer);
            }
            else
            {
                base.Write(writer);
            }
        }
    }
}
namespace mtanksl.ActionMessageFormat
{
    [TraitClass("flex.messaging.messages.AsyncMessageExt")]
    [TraitClass("DSA")]
    public class AsyncMessageExt : AsyncMessage, IExternalizable
    {
        private AsyncMessage asyncMessage;

        public AsyncMessageExt()
        {

        }

        public AsyncMessageExt(AsyncMessage asyncMessage)
        {
            this.asyncMessage = asyncMessage;
        }

        public override void Read(AmfReader reader)
        {
            if (asyncMessage != null)
            {
                asyncMessage.Read(reader);
            }
            else
            {
                base.Read(reader);
            }
        }

        public override void Write(AmfWriter writer)
        {
            if (asyncMessage != null)
            {
                asyncMessage.Write(writer);
            }
            else
            {
                base.Write(writer);
            }
        }
    }
}
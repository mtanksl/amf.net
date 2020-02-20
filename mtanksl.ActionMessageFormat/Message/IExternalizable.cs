namespace mtanksl.ActionMessageFormat
{
    public interface IExternalizable
    {
        void Read(AmfReader reader);

        void Write(AmfWriter writer);
    }
}
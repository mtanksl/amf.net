namespace mtanksl.ActionMessageFormat
{
    public interface IAmfObject
    {
        void FromObject(object value);

        object ToObject();
    }
}
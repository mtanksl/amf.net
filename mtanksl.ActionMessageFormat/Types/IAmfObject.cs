namespace mtanksl.ActionMessageFormat
{
    public interface IAmfObject
    {
        T ToObject<T>();

        T ToObject<T>(IAmfSerializer serializer);

        object ToObject();

        object ToObject(IAmfSerializer serializer);       
    }
}
namespace mtanksl.ActionMessageFormat
{
    public interface IAmfObject
    {
        T ToObject<T>();

        T ToObject<T>(AmfSerializer serializer);

        object ToObject();

        object ToObject(AmfSerializer serializer);       
    }
}
namespace mtanksl.ActionMessageFormat
{
    public interface IAmfObject
    {
        object ToObject();

        object ToObject(AmfSerializer serializer);
    }
}
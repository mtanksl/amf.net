namespace mtanksl.ActionMessageFormat
{
    public interface IAmfSerializer
    {
        bool ThrowIfPropertyNotFound { get; set; }

        object Normalize(object value);
    }
}
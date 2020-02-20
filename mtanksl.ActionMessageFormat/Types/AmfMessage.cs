namespace mtanksl.ActionMessageFormat
{
    public class AmfMessage
    {
        public string TargetUri { get; set; }

        public string ResponseUri { get; set; }

        public object Data { get; set; }
    }
}
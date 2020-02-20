namespace mtanksl.ActionMessageFormat
{
    public class AmfHeader
    {
        public string Name { get; set; }

        public bool MustUnderstand { get; set; }

        public object Data { get; set; }
    }
}
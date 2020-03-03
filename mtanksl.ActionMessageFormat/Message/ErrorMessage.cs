namespace mtanksl.ActionMessageFormat
{
    [TraitClass("flex.messaging.messages.ErrorMessage")]
    public class ErrorMessage : AsyncMessage
    {
        [TraitMember("faultCode")]
        public string FaultCode { get; set; }

        [TraitMember("faultString")]
        public string FaultString { get; set; }

        [TraitMember("faultDetail")]
        public string FaultDetail { get; set; }

        [TraitMember("rootCause")]
        public object RootCause { get; set; }

        [TraitMember("extendedData")]
        public object ExtendedData { get; set; }

        public override void Read(AmfReader reader)
        {
            base.Read(reader);

            
        }

        public override void Write(AmfWriter writer)
        {
            base.Write(writer);

            
        }
    }
}
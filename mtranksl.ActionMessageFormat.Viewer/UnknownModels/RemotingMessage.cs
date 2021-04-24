using mtanksl.ActionMessageFormat;

namespace mtranksl.ActionMessageFormat.Viewer.UnknownModels
{
    [TraitClass("flex.messaging.messages.RemotingMessage")]
    public class RemotingMessage
    {
        [TraitMember("operation")]
        public object Operation { get; set; } //

        [TraitMember("source")]
        public object Source { get; set; }

        [TraitMember("timestamp")]
        public object Timestamp { get; set; }

        [TraitMember("destination")]
        public object Destination { get; set; } //

        [TraitMember("body")]
        public object Body { get; set; }

        [TraitMember("clientId")]
        public object ClientId { get; set; }

        [TraitMember("headers")]
        public object Headers { get; set; }

        [TraitMember("messageId")]
        public object MessageId { get; set; }

        [TraitMember("timeToLive")]
        public object TimeToLive { get; set; }
    }
}
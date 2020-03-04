namespace mtanksl.ActionMessageFormat
{
    public interface IMessage
    {
        object Body { get; set; }

        string ClientId { get; set; }

        byte[] ClientIdBytes { get; set; }

        string Destination { get; set; }

        object Headers { get; set; }

        string MessageId { get; set; }

        byte[] MessageIdBytes { get; set; }

        double Timestamp { get; set; }

        double TimeToLive { get; set; }
    }
}
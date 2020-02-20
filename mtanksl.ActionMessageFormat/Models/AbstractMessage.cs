using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat
{
    public class AbstractMessage : IExternalizable
    {
        [TraitMember("body")]
        public object Body { get; set; }

        [TraitMember("clientId")]
        public string ClientId { get; set; }

        [TraitMember("clientIdBytes")]
        public List<byte> ClientIdBytes { get; set; }

        [TraitMember("destination")]
        public string Destination { get; set; }

        [TraitMember("headers")]
        public object Headers { get; set; }

        [TraitMember("messageId")]
        public string MessageId { get; set; }

        [TraitMember("messageIdBytes")]
        public List<byte> MessageIdBytes { get; set; }

        [TraitMember("timestamp")]
        public double Timestamp { get; set; }

        [TraitMember("timeToLive")]
        public double TimeToLive { get; set; }

        public virtual void Read(AmfReader reader)
        {
            var flags = reader.ReadFlags();

            for (int i = 0; i < flags.Count; i++)
            {
                var flag = flags[i];
                
                if (i == 0)
                {
                    if ( (flag & 1) != 0)
                    {
                        Body = reader.ReadAmf3();
                    }

                    if ( (flag & 2) != 0)
                    {
                        ClientId = (string)reader.ReadAmf3();
                    }

                    if ( (flag & 4) != 0)
                    {
                        Destination = (string)reader.ReadAmf3();
                    }

                    if ( (flag & 8) != 0)
                    {
                        Headers = reader.ReadAmf3();
                    }

                    if ( (flag & 16) != 0)
                    {
                        MessageId = (string)reader.ReadAmf3();
                    }

                    if ( (flag & 32) != 0)
                    {
                        Timestamp = (double)reader.ReadAmf3();
                    }

                    if ( (flag & 64) != 0)
                    {
                        TimeToLive = (double)reader.ReadAmf3();
                    }
                }
                else if (i == 1)
                {
                    if ( (flag & 1) != 0)
                    {
                        ClientIdBytes = (List<byte>)reader.ReadAmf3();
                    }

                    if ( (flag & 2) != 0)
                    {
                        MessageIdBytes = (List<byte>)reader.ReadAmf3();
                    }
                }
            }
        }

        public virtual void Write(AmfWriter writer)
        {
            byte flag = 0;

            byte flag2 = 0;

            if (Body != null)
            {
                flag |= 1;
            }

            if (ClientId != null)
            {
                flag |= 2;
            }

            if (Destination != null)
            {
                flag |= 4;
            }

            if (Headers != null)
            {
                flag |= 8;
            }

            if (MessageId != null)
            {
                flag |= 16;
            }

            if (Timestamp > 0)
            {
                flag |= 32;
            }

            if (TimeToLive > 0)
            {
                flag |= 64;
            }

            if (ClientIdBytes != null)
            {
                flag2 |= 1;
            }

            if (MessageIdBytes != null)
            {
                flag2 |= 2;
            }

            if (flag2 == 0)
            {
                writer.WriteByte(flag);
            }
            else
            {
                writer.WriteByte( (byte)(flag | 128) );

                writer.WriteByte(flag2);
            }

            if (Body != null)
            {
                writer.WriteAmf3(Body);
            }

            if (ClientId != null)
            {
                writer.WriteAmf3(ClientId);
            }

            if (Destination != null)
            {
                writer.WriteAmf3(Destination);
            }

            if (Headers != null)
            {
                writer.WriteAmf3(Headers);
            }

            if (MessageId != null)
            {
                writer.WriteAmf3(MessageId);
            }

            if (Timestamp > 0)
            {
                writer.WriteAmf3(Timestamp);
            }

            if (TimeToLive > 0)
            {
                writer.WriteAmf3(TimeToLive);
            }

            if (ClientIdBytes != null)
            {
                writer.WriteAmf3(ClientIdBytes);
            }

            if (MessageIdBytes != null)
            {
                writer.WriteAmf3(MessageIdBytes);
            }
        }
    }
}
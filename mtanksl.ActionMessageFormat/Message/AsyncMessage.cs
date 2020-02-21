namespace mtanksl.ActionMessageFormat
{
    [TraitClass("DSA")]
    [TraitClass("flex.messaging.messages.AsyncMessage")]
    [TraitClass("flex.messaging.messages.AsyncMessageExt")]
    public class AsyncMessage : AbstractMessage
    {
        [TraitMember("correlationId")]
        public string CorrelationId { get; set; }

        [TraitMember("correlationIdBytes")]
        public byte[] CorrelationIdBytes { get; set; }

        public override void Read(AmfReader reader)
        {
            base.Read(reader);

            var flags = reader.ReadFlags();

            for (int i = 0; i < flags.Count; i++)
            {
                var flag = flags[i];

                if (i == 0)
                {
                    if ( (flag & 1) != 0)
                    {
                        CorrelationId = (string)reader.ReadAmf3();
                    }

                    if ( (flag & 2) != 0)
                    {
                        CorrelationIdBytes = (byte[])reader.ReadAmf3();
                    }
                }
            }
        }

        public override void Write(AmfWriter writer)
        {
            base.Write(writer);

            byte flag = 0;

            if (CorrelationId != null)
            {
                flag |= 1;
            }

            if (CorrelationIdBytes != null)
            {
                flag |= 2;
            }

            writer.WriteByte(flag);

            if (CorrelationId != null)
            {
                writer.WriteAmf3(CorrelationId);
            }

            if (CorrelationIdBytes != null)
            {
                writer.WriteAmf3(CorrelationIdBytes);
            }
        }
    }
}
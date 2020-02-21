namespace mtanksl.ActionMessageFormat
{
    [TraitClass("DSC")]
    [TraitClass("flex.messaging.messages.CommandMessage")]
    [TraitClass("flex.messaging.messages.CommandMessageExt")]
    public class CommandMessage : AsyncMessage
    {
        [TraitMember("operation")]
        public int Operation { get; set; }

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
                        Operation = (int)reader.ReadAmf3();
                    }
                }
            }
        }

        public override void Write(AmfWriter writer)
        {
            base.Write(writer);

            byte flag = 0;

            if (Operation > 0)
            {
                flag |= 1;
            }

            writer.WriteByte(flag);

            if (Operation > 0)
            {
                writer.WriteAmf3(Operation);
            }
        }
    }
}
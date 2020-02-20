using System.Collections.Generic;

namespace mtanksl.ActionMessageFormat
{
    public class AmfPacket
    {
        public AmfVersion Version { get; set; }

        public List<AmfHeader> Headers { get; set; }

        public List<AmfMessage> Messages { get; set; }
    }
}
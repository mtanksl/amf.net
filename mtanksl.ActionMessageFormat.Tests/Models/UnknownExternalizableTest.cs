using System;
using System.Text;

namespace mtanksl.ActionMessageFormat.Tests
{
    [TraitClass("unknownExternalizableTest")]
    public class UnknownExternalizableTest : IExternalizable
    {
        public string UnknownBytes { get; set; }

        public void Write(AmfWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Read(AmfReader reader)
        {
            var unknownBytes = new StringBuilder();

            while (reader.CanReadByte() )
            {
                byte anything = reader.ReadByte();

                unknownBytes.AppendFormat("{0:X2} ", anything);
            }

            UnknownBytes = unknownBytes.ToString();
        }
    }
}
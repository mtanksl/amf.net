namespace mtanksl.ActionMessageFormat.Tests
{
    [TraitClass("test")]
    public class Test
    {
        [TraitMember("byte")]
        public byte Byte { get; set; }

        [TraitMember("false")]
        public bool False { get; set; }

        [TraitMember("true")]
        public bool True { get; set; }

        [TraitMember("short")]
        public short Short { get; set; }

        [TraitMember("int")]
        public int Int { get; set; }

        [TraitMember("double")]
        public double Double { get; set; }

        [TraitMember("string")]
        public string String { get; set; }

        [TraitMember("reference")]
        public Test Reference { get; set; }
    }
}
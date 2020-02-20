namespace mtanksl.ActionMessageFormat
{
    public enum Amf3Type : byte
    {
        Undefined = 0,

        Null = 1,

        BooleanFalse = 2,

        BooleanTrue = 3,

        Integer = 4,

        Double = 5,

        String = 6,

        XmlDocument = 7,

        Date = 8,

        Array = 9,

        Object = 10,

        Xml = 11,

        ByteArray = 12,

        VectorInt = 13,

        VectorUInt = 14,

        VectorDouble = 15,

        VectorObject = 16,

        Dictionary = 17
    }
}
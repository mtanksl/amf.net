namespace mtanksl.ActionMessageFormat
{
    public enum Amf0Type : byte
    {
        Number = 0,

        Boolean = 1,

        String = 2,

        Object = 3,

        //Movieclicp = 4,

        Null = 5,

        Undefined = 6,

        Reference = 7,

        EcmaArray = 8,

        ObjectEnd = 9,

        StrictArray = 10,

        Date = 11,

        LongString = 12,

        Unsuported = 13,

        //RecordSet = 14,

        XMLDocument = 15,

        TypedObject = 16,

        Amf3 = 17
    }
}
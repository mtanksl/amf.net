# AMF.net

An implementation in C# of both AMF0 and AMF3 binary format that is used to serialize ActionScript object graphs.

# Why?

So, Flash is dead and Adobe will end support of Flash Player on December 31, 2020. 
I actually never used Flash and had no knowledge of it's build in communication protocol.
It turns out that I had to automate a creepy old software written in Flash.
Since I did not found any good library, I've written my own.

# Audience

Probably nobody. 
But hey, It is fun to learn and implement a protocol. 
I kind liked this one (but no, I will never use it again).

# Specification

Here is the official [Action Message Format AMF0 Specification](https://www.adobe.com/content/dam/acom/en/devnet/pdf/amf0-file-format-specification.pdf) and [Action Message Format AMF3 Specification](https://www.adobe.com/content/dam/acom/en/devnet/pdf/amf-file-format-spec.pdf).

# How to serialize an object

```C#
var writer = new AmfWriter();

writer.WriteAmfPacket(new AmfPacket()
{
    Version = AmfVersion.Amf3,
    Headers = new List<AmfHeader>(),
    Messages = new List<AmfMessage>()
    {
        new AmfMessage()
        {
            TargetUri = "null",
            ResponseUri = "/1",
            Data = new CommandMessageExt()
            {
                Operation = 5,
                CorrelationId = "",
                TimeToLive = 0,
                Timestamp = 0,
                Headers = new { DSMessagingVersion = 1, DSId = "nil" },
                Body = new { },
                ClientId = null,
                Destination = "",
                MessageId = Guid.NewGuid().ToString()
            }
        }
    }
} );
```

# How to deserialize an object

```C#
var reader = new AmfReader(writer.Data);

var packet = reader.ReadAmfPacket();
```

# Object Viewer

I've also implemented a custom object viewer. Please note that IExternalizable classes need the definition models within the project. Use [TraitClass] and [TraitMember] attributes to map those classes and properties.

![Object Viewer][/ObjectViewer.png]
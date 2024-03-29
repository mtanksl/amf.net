# AMF.net

An implementation in C# of both AMF0 and AMF3 binary format that is used to serialize ActionScript object graphs.

# Install using NuGet
![Nuget](https://img.shields.io/nuget/v/mtanksl.ActionMessageFormat)

```
dotnet add package mtanksl.ActionMessageFormat --version 1.0.4
```

# Why?

So, Flash is dead and Adobe will end support of Flash Player on December 31, 2020. 
I actually never used Flash and had no knowledge of it's build in communication protocol.
It turns out that I had to automate a creepy old software written in Flash.
Since I did not find any good library, I've written my own.

# Audience

Probably nobody. 
But hey, It is fun to learn and implement a protocol. 
I kind liked this one (but no, I will never use it again).

# Specification

Here is the official ![Action Message Format AMF0 Specification](/amf0.pdf) and ![Action Message Format AMF3 Specification](/amf3.pdf).

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

I've also implemented a custom object viewer. 
Please note that IExternalizable classes need the definition models within the project. 
Use [TraitClass] and [TraitMember] attributes to map custom classes and properties.

![Object Viewer](/ObjectViewer.png)

# Fiddler Inspector

I've also implemented a custom Fiddler Inspector. 
Please note that IExternalizable classes need the definition models within the project. 
Use [TraitClass] and [TraitMember] attributes to map custom classes and properties.
To build, add a reference to C:\Users\<USER>\AppData\Local\Programs\Fiddler\Fiddler.exe
After build, copy all dlls to C:\Users\<USER>\Documents\Fiddler2\Inspectors\

![Fiddler Inspector](/FiddlerInspector.png)

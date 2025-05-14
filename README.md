# KazusaProtoBuf

Flexible and schema-less Protobuf parser + message builder, made to be easily embdded into your code.\
Kazusa doesn't like installing many libraries, so this one only uses Newtonsoft.Json (for message->JSON serializing)

## Features

- Parses raw Protobuf messages without schema
- Generates `.proto`-style schema from data
- Dynamically builds new messages at runtime
- Converts messages to JSON for visual inspection

## Example

```csharp
using KazusaProtoBuf.ProtoModule;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1) return;

        byte[] rawData = Convert.FromBase64String(args[0]);
        ProtoMessage msg = ProtoMessage.FromByteArray(rawData);

        Console.WriteLine(msg.GenerateSchema());
        Console.WriteLine();
        Console.WriteLine(msg.ToJson());
    }
}

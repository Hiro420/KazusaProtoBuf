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
    public static void Main(string[] args)
    {
        if (args.Length < 1) return;

        // ----------------------------------------
        // Decode a message dynamically
        // ----------------------------------------

        // Decode base64 input
        byte[] rawData = Convert.FromBase64String(args[0]);

        // Parse message
        ProtoMessage msg = ProtoMessage.FromByteArray(rawData);

        // Output inferred schema
        Console.WriteLine(msg.GenerateSchema());
        Console.WriteLine();

        // Output JSON representation
        Console.WriteLine(msg.ToJson());

        // ----------------------------------------
        // Construct a message dynamically
        // ----------------------------------------

        Console.WriteLine(Environment.NewLine);

        ProtoMessage message = new ProtoMessage();

        message.AddField(1, 42);
        message.AddField(2, true);
        message.AddField(3, 123.456f);
        message.AddField(4, 789.101112);
        message.AddField(5, "Kazusa best girl");
        message.AddField(6, new byte[] { 0xCA, 0xFE });
        message.AddField(7, new List<int> { 1, 2, 3 });
        message.AddField(8, new uint[] { 4, 5, 6 });

        var nested = new ProtoMessage();
        nested.AddField(1, "Nested Kazusa");
        nested.AddField(2, 99);

        message.AddField(9, nested);
        message.AddField(10, new List<ProtoMessage> { nested, nested, nested });

        byte[] encoded = message.ToByteArray();
        Console.WriteLine(Convert.ToBase64String(encoded));

        string generatedSchema = message.GenerateSchema("KazusaProto");
        Console.WriteLine(generatedSchema);
    }
}
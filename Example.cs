using KazusaProtoBuf.ProtoModule;

namespace KazusaProtoBuf;

internal class Example
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            return;
        }

        // Convert the Base64 string from arguments into a byte array
        byte[] rawData = Convert.FromBase64String(args[0]);

        // Alternatively, read the Base64 string from a file
        // byte[] rawData = Convert.FromBase64String(File.ReadAllText("b64.txt"));

        // Parse the byte array into a ProtoMessage
        ProtoMessage msg = ProtoMessage.FromByteArray(rawData);

        // --- Output the generated proto3 schema ---
        Console.WriteLine(msg.GenerateSchema());

        Console.WriteLine(); // Spacer for readability

        // --- Output the message in JSON format for visual inspection ---
        Console.WriteLine(msg.ToJson());


        // ----------------------------------------
        // Construct a message dynamically
        // ----------------------------------------


        Console.WriteLine(Environment.NewLine); // Spacer for readability

        ProtoMessage message = new ProtoMessage();

        message.AddField(1, 42);                        // int32
        message.AddField(2, true);                      // bool
        message.AddField(3, 123.456f);                  // float
        message.AddField(4, 789.101112);                // double
        message.AddField(5, "Kazusa best girl");        // string
        message.AddField(6, new byte[] { 0xCA, 0xFE }); // bytes
        message.AddField(7, new List<int> { 1, 2, 3 }); // repeated int
        message.AddField(8, new uint[] { 4, 5, 6 });    // repeated int

        // Nested message
        ProtoMessage nested = new ProtoMessage();
        nested.AddField(1, "Nested Kazusa");
        nested.AddField(2, 99);

        message.AddField(9, nested);
        message.AddField(10, new List<ProtoMessage> { nested, nested, nested });

        byte[] bytes = message.ToByteArray();
        Console.WriteLine(Convert.ToBase64String(bytes));

        string schema = message.GenerateSchema("KazusaProto");
        Console.WriteLine(schema);

    }
}

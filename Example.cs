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
    }
}

using System.Text;

namespace KazusaProtoBuf.ProtoModule;

internal static class ProtoDeserializer
{
    public static ulong ReadVarint(BinaryReader reader)
    {
        ulong result = 0;
        int shift = 0;

        while (true)
        {
            byte b = reader.ReadByte();
            result |= (ulong)(b & 0x7F) << shift;

            if ((b & 0x80) == 0) break;

            shift += 7;
            if (shift > 64)
                throw new FormatException("Varint too long");
        }

        return result;
    }

    public static object ReadLengthDelimited(BinaryReader reader)
    {
        ulong length = ReadVarint(reader);
        byte[] bytes = reader.ReadBytes((int)length);

        try
        {
            string str = Encoding.UTF8.GetString(bytes);
            if (str.All(c => c >= 0x20 || c == '\n' || c == '\r' || c == '\t'))
                return str;
        }
        catch { }

        try
        {
            return ProtoMessage.FromByteArray(bytes);
        }
        catch
        {
            return bytes;
        }
    }
}

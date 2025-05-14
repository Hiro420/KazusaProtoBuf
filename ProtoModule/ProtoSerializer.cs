using System.Text;

namespace KazusaProtoBuf.ProtoModule;

internal static class ProtoSerializer
{
    public static void WriteField(Stream stream, int tag, WireType wireType, object value)
    {
        uint header = ((uint)tag << 3) | (uint)wireType;
        WriteVarint(stream, header);

        switch (wireType)
        {
            case WireType.Varint:
                WriteVarint(stream, Convert.ToUInt64(value));
                break;
            case WireType.Fixed64:
                stream.Write(BitConverter.GetBytes(Convert.ToDouble(value)), 0, 8);
                break;
            case WireType.LengthDelimited:
                if (value is string str)
                {
                    var bytes = Encoding.UTF8.GetBytes(str);
                    WriteVarint(stream, (uint)bytes.Length);
                    stream.Write(bytes, 0, bytes.Length);
                }
                else if (value is byte[] byteArr)
                {
                    WriteVarint(stream, (uint)byteArr.Length);
                    stream.Write(byteArr, 0, byteArr.Length);
                }
                else if (value is ProtoMessage nested)
                {
                    var nestedBytes = nested.ToByteArray();
                    WriteVarint(stream, (uint)nestedBytes.Length);
                    stream.Write(nestedBytes, 0, nestedBytes.Length);
                }
                break;
            case WireType.Fixed32:
                stream.Write(BitConverter.GetBytes(Convert.ToSingle(value)), 0, 4);
                break;
            default:
                throw new NotSupportedException($"Unsupported wire type: {wireType}");
        }
    }

    public static (WireType, object) GetWireTypeAndValue(object value) => value switch
    {
        int or uint or long or ulong or bool => (WireType.Varint, Convert.ToUInt64(value)),
        float => (WireType.Fixed32, value),
        double => (WireType.Fixed64, value),
        string or byte[] or ProtoMessage => (WireType.LengthDelimited, value),
        _ => throw new NotSupportedException($"Unsupported value type: {value.GetType().Name}")
    };

    public static void WriteVarint(Stream stream, ulong value)
    {
        while (value >= 0x80)
        {
            stream.WriteByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }
        stream.WriteByte((byte)value);
    }

    public static object ConvertValue(object value) => value switch
    {
        ProtoMessage msg => Newtonsoft.Json.JsonConvert.DeserializeObject(msg.ToJson())!,
        byte[] bytes => Convert.ToBase64String(bytes),
        _ => value
    };
}

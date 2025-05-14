using Newtonsoft.Json;

namespace KazusaProtoBuf.ProtoModule;

public class ProtoMessage
{
    public List<ProtoField> _fields = new();

    public void AddField<T>(int tag, T value)
    {
        if (value is System.Collections.IEnumerable enumerable && value is not string && value is not byte[])
        {
            foreach (var item in enumerable)
            {
                AddField(tag, item);
            }
            return;
        }

        AddField(tag, (object)value!);
    }

    private void AddField(int tag, object value)
    {
        var (wireType, actualValue) = ProtoSerializer.GetWireTypeAndValue(value);
        _fields.Add(new ProtoField(tag, wireType, actualValue));
    }

    public byte[] ToByteArray()
    {
        using var ms = new MemoryStream();
        foreach (var field in _fields)
        {
            ProtoSerializer.WriteField(ms, field.Tag, field.WireType, field.Value);
        }
        return ms.ToArray();
    }

    public string ToJson()
    {
        var dict = new Dictionary<string, object>();

        foreach (var field in _fields)
        {
            string key = $"field{field.Tag}";
            object converted = ProtoSerializer.ConvertValue(field.Value);

            if (dict.TryGetValue(key, out var existing))
            {
                if (existing is List<object> list)
                    list.Add(converted);
                else
                    dict[key] = new List<object> { existing, converted };
            }
            else
            {
                dict[key] = converted;
            }
        }

        return JsonConvert.SerializeObject(dict, Formatting.Indented);
    }

    public string GenerateSchema(string messageName = "GeneratedMessage", int indentLevel = 0)
    {
        return ProtoSchemaGenerator.Generate(_fields, messageName, indentLevel);
    }

    public static ProtoMessage FromByteArray(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        var message = new ProtoMessage();

        while (ms.Position < ms.Length)
        {
            ulong key = ProtoDeserializer.ReadVarint(reader);
            int tag = (int)(key >> 3);
            WireType wireType = (WireType)(key & 0x07);

            object value = wireType switch
            {
                WireType.Varint => ProtoDeserializer.ReadVarint(reader),
                WireType.Fixed64 => reader.ReadDouble(),
                WireType.LengthDelimited => ProtoDeserializer.ReadLengthDelimited(reader),
                WireType.Fixed32 => reader.ReadSingle(),
                _ => throw new NotSupportedException($"Unsupported wire type: {wireType}")
            };

            message.AddField(tag, value);
        }

        return message;
    }
}

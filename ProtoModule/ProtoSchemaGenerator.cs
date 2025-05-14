using System.Text;

namespace KazusaProtoBuf.ProtoModule;

internal static class ProtoSchemaGenerator
{
    public static string Generate(List<ProtoField> fields, string messageName, int indentLevel)
    {
        var sb = new StringBuilder();
        string indent = new string(' ', indentLevel * 2);
        sb.AppendLine($"{indent}message {messageName} {{");

        var tagToField = new Dictionary<int, ProtoField>();
        var tagCounts = new Dictionary<int, int>();
        var nestedSchemas = new List<string>();
        var seenNested = new HashSet<int>();

        foreach (var field in fields)
        {
            tagToField[field.Tag] = field;
            tagCounts.TryAdd(field.Tag, 0);
            tagCounts[field.Tag]++;
        }

        foreach (var kv in tagToField.OrderBy(f => f.Key))
        {
            int tag = kv.Key;
            var field = kv.Value;
            bool repeated = tagCounts[tag] > 1;
            string nestedName = $"NestedMessage{tag}";
            string fieldType = GetProtoType(field.Value, nestedName, out string? nestedSchema, indentLevel + 1);

            string repeatedStr = repeated ? "repeated " : "";
            sb.AppendLine($"{indent}  {repeatedStr}{fieldType} field{tag} = {tag};");

            if (nestedSchema != null && seenNested.Add(tag))
            {
                sb.AppendLine();
                sb.Append(nestedSchema);
            }
        }

        sb.AppendLine($"{indent}}}\n");
        return sb.ToString();
    }

    private static string GetProtoType(object value, string nestedName, out string? nestedSchema, int indentLevel)
    {
        nestedSchema = null;
        var type = value.GetType();

        if (value is int or long) return "int32";
        if (value is uint or ulong) return "uint32";
        if (value is float) return "float";
        if (value is double) return "double";
        if (value is bool) return "bool";
        if (value is string) return "string";
        if (value is byte[]) return "bytes";

        if (value is ProtoMessage nested)
        {
            nestedSchema = Generate(nested._fields, nestedName, indentLevel);
            return nestedName;
        }

        throw new NotSupportedException($"Unsupported type for schema: {type.Name}");
    }
}

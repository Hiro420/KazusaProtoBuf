namespace KazusaProtoBuf.ProtoModule;

public class ProtoField
{
    public int Tag { get; set; }
    public WireType WireType { get; set; }
    public object Value { get; set; }
    public Type? FieldType => Value?.GetType();

    public ProtoField(int tag, WireType wireType, object value)
    {
        Tag = tag;
        WireType = wireType;
        Value = value;
    }
}

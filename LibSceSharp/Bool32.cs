namespace LibSceSharp;

public struct Bool32
{
    private int _value;
    
    public static implicit operator Bool32(bool val) => new() { _value = val ? 1 : 0 };
    public static implicit operator bool(Bool32 val) => val._value != 0;
}

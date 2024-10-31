namespace LibSceSharp;

public struct Bool32
{
    private int _value;

    [Obsolete("Using the constructor of Bool32 directly is unsupported! Bool32 can coerce back and forth from bool")]
    // ReSharper disable once UnusedMember.Global
    public Bool32() {}
    
    private Bool32(int value) => _value = value;
    
    public static implicit operator Bool32(bool val) => new(val ? 1 : 0);

    public static implicit operator bool(Bool32 val) => val._value != 0;
}
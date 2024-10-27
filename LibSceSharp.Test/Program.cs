using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibSceSharp.Test;

static class Program
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void LogOverride(byte* scopePtr, LibSceLogLevel level, byte* messagePtr)
    {
        var scope = Marshal.PtrToStringUTF8((IntPtr)scopePtr);
        var message = Marshal.PtrToStringUTF8((IntPtr)messagePtr);
        
        Console.WriteLine("override log func, ({0}) {1}: {2}", scope, level, message);
    } 
    
    static unsafe void Main(string[] args)
    {
        LibSce.SetLogCallback(&LogOverride);
        
        using var libsce = new LibSce();

        var cfData = File.ReadAllBytes(args[0]);
        
        var contentId = libsce.GetContentId(cfData);
        
        Console.WriteLine($"Content ID: \"{contentId}\"");
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibSceSharp.Test;

internal static class Program
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void LogOverride(byte* scopePtr, LibSceLogLevel level, byte* messagePtr)
    {
        var scope = Marshal.PtrToStringUTF8((IntPtr)scopePtr);
        var message = Marshal.PtrToStringUTF8((IntPtr)messagePtr);

        Console.WriteLine("override log func, ({0}) {1}: {2}", scope, level, message);
    }

    private static unsafe void Main(string[] args)
    {
        LibSce.SetLogCallback(&LogOverride);

        var libsce = new LibSce();
        var self = new Self(libsce, args[0]);

        Console.WriteLine($"Content ID: \"{self.ContentId}\", is npdrm: {self.IsNpdrmApplication}");
        
        self.Dispose();
        libsce.Dispose();
    }
}
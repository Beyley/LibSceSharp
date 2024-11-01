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
        var self = new Self(libsce, File.ReadAllBytes(args[0]), true);

        Console.WriteLine($"Content ID: \"{self.ContentId}\", needs npdrm license: {self.NeedsNpdrmLicense}, load status: {self.LoadStatus}");
        
        self.Dispose();
        libsce.Dispose();
    }
}
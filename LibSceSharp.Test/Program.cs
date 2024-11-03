using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibSceSharp.Test;

internal static class Program
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void LogOverride(byte* scopePtr, LibSceLogLevel level, byte* messagePtr)
    {
        string? scope = Marshal.PtrToStringUTF8((IntPtr)scopePtr);
        string? message = Marshal.PtrToStringUTF8((IntPtr)messagePtr);

        Console.WriteLine("override log func, ({0}) {1}: {2}", scope, level, message);
    }

    private static unsafe void Main(string[] args)
    {
        LibSce.SetLogCallback(&LogOverride);

        LibSce libsce = new();
        Self self = new(libsce, File.ReadAllBytes(args[0]), false);

        Console.WriteLine($"Content ID: \"{self.ContentId}\", needs npdrm license: {self.NeedsNpdrmLicense}, load status: {self.LoadStatus}");

        Span<byte> elf = self.ExtractToElf();

        using FileStream handle = File.OpenWrite("out.elf");
        handle.Write(elf);
        handle.Flush();
        
        libsce.FreeMemory(elf);
        
        self.Dispose();
        libsce.Dispose();
    }
}
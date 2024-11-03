using System.Runtime.InteropServices;

namespace LibSceSharp;

public unsafe class LibSce : IDisposable
{
    private readonly void* _handle;
    

    public static implicit operator void*(LibSce val) => val._handle;

    public LibSce()
    {
        LibSceNative.HandleError(LibSceNative.libsce_create(out _handle), nameof(LibSceNative.libsce_create));
    }

    public static void SetLogCallback(delegate* unmanaged[Cdecl]<byte*, LibSceLogLevel, byte*, void> ptr)
    {
        LibSceNative.libsce_set_log_callback(ptr);
    }

    public void FreeMemory(Span<byte> memory)
    {
        fixed (byte* memoryPtr = memory)
            LibSceNative.libsce_free_memory(this, memoryPtr, (UIntPtr)memory.Length);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        LibSceNative.HandleError(LibSceNative.libsce_destroy(_handle), nameof(LibSceNative.libsce_destroy));
    }

    ~LibSce()
    {
        ReleaseUnmanagedResources();
    }
}
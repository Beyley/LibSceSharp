using System.Runtime.InteropServices;
using System.Text;

namespace LibSceSharp;

public unsafe class LibSce : IDisposable
{
    private readonly void* _handle;
    
    public static void SetLogCallback(delegate* unmanaged[Cdecl]<byte*, LibSceLogLevel, byte*, void> ptr)
    {
        LibSceNative.libsce_set_log_callback(ptr);
    }
    
    public LibSce()
    {
        LibSceNative.HandleError(LibSceNative.libsce_create(out _handle), nameof(LibSceNative.libsce_create));
    }

    public string? GetContentId(byte[] cfData)
    {
        const int contentIdSize = 0x30;

        var contentId = stackalloc byte[contentIdSize];

        fixed (byte* cfDataFixed = cfData)
        {
            var ret = LibSceNative.libsce_get_content_id(_handle, cfDataFixed, (nuint)cfData.LongLength, contentId);

            if (ret == LibSceNative.NoContentIdError)
                return null;

            LibSceNative.HandleError(ret, nameof(LibSceNative.libsce_get_content_id));
        }
        
        return Marshal.PtrToStringAnsi((IntPtr)contentId);
    }

    private void ReleaseUnmanagedResources()
    {
        LibSceNative.HandleError(LibSceNative.libsce_destroy(_handle), nameof(LibSceNative.libsce_destroy));
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    
    ~LibSce()
    {
        ReleaseUnmanagedResources();
    }
}
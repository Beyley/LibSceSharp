using System.Runtime.InteropServices;
using System.Text;

namespace LibSceSharp;

public unsafe class LibSce : IDisposable
{
    private readonly void* _handle;
    
    public LibSce()
    {
        Abi.HandleError(Abi.libsce_create(out _handle), nameof(Abi.libsce_create));
    }

    public string? GetContentId(byte[] cfData)
    {
        const int contentIdSize = 0x30;

        var contentId = stackalloc byte[contentIdSize];

        fixed (byte* cfDataFixed = cfData)
        {
            var ret = Abi.libsce_get_content_id(_handle, cfDataFixed, (nuint)cfData.LongLength, contentId);

            if (ret == Abi.NoContentIdError)
                return null;

            Abi.HandleError(ret, nameof(Abi.libsce_get_content_id));
        }
        
        return Marshal.PtrToStringAnsi((IntPtr)contentId);
    }

    private void ReleaseUnmanagedResources()
    {
        Abi.HandleError(Abi.libsce_destroy(_handle), nameof(Abi.libsce_destroy));
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
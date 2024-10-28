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

    /// <summary>
    /// Reads the content ID out of the passed SELF file
    /// </summary>
    /// <param name="path">The path to the SELF file</param>
    /// <returns>The read content ID</returns>
    public string? GetContentId(string path) 
        => GetContentId(File.ReadAllBytes(path));

    /// <summary>
    /// Reads the content ID out of the passed SELF
    /// </summary>
    /// <remarks>
    /// This function *will* mutate the contents of cfData! If this is bad for you, please make a copy before calling this function
    /// </remarks>
    /// <param name="cfData">The CF data</param>
    /// <returns>The read content ID</returns>
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
    
    /// <summary>
    /// Checks whether the passed SELF file is an NPDRM application or not
    /// </summary>
    /// <param name="path">The path to the SELF file</param>
    /// <returns>Whether the SELF file is an NPDRM application</returns>
    public bool IsSelfNpdrm(string path) 
        => IsSelfNpdrm(File.ReadAllBytes(path));

    /// <summary>
    /// Checks whether the passed SELF is an NPDRM application or not
    /// </summary>
    /// <param name="cfData">The CF data</param>
    /// <returns>Whether the SELF file is an NPDRM application</returns>
    public bool IsSelfNpdrm(byte[] cfData)
    {
        fixed (byte* cfDataFixed = cfData)
        {
            var ret = LibSceNative.libsce_is_self_npdrm(_handle, cfDataFixed, (nuint)cfData.LongLength, out var isNpdrm);

            LibSceNative.HandleError(ret, nameof(LibSceNative.libsce_get_content_id));

            return isNpdrm;
        }
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
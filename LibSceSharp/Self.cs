using System.Runtime.InteropServices;

namespace LibSceSharp;

public unsafe class Self : IDisposable
{
    private readonly void* _handle;
    private readonly LibSce _libSce;
    
    private readonly byte* _dataPtr;
    private readonly nuint _dataLen;

    public SelfLoadStatus LoadStatus => LibSceNative.libsce_self_get_load_status(_handle);
    
    public bool IsNpdrmApplication => LibSceNative.libsce_self_is_npdrm_application(_handle);

    public string? ContentId
    {
        get
        {
            const int contentIdSize = 0x30;

            var contentId = stackalloc byte[contentIdSize];

            return LibSceNative.libsce_self_get_content_id(_handle, contentId)
                ? Marshal.PtrToStringAnsi((IntPtr)contentId)
                : null;
        }
    }

    public Self(LibSce libsce, string path) : this(libsce, File.ReadAllBytes(path)) {}

    public Self(LibSce libsce, byte[] data)
    {
        // NOTE: we have to duplicate the memory here since libsce expects to take ownership of the passed memory (but is not responsible for freeing it!)
        
        _libSce = libsce;
        
        // Allocate some static memory for the SELF data
        _dataLen = (UIntPtr)data.LongLength;
        _dataPtr = (byte*)NativeMemory.Alloc(_dataLen);
        
        // Copy the data in
        data.AsSpan().CopyTo(new Span<byte>(_dataPtr, data.Length));

        try
        {
            // Load the SELF file
            LibSceNative.HandleError(LibSceNative.libsce_self_load(libsce, _dataPtr, _dataLen, out _handle), nameof(LibSceNative.libsce_self_load));
        }
        catch
        {
            // If we fail, free the memory and exit out
            NativeMemory.Free(_dataPtr);
            throw;
        }
    }

    private void ReleaseUnmanagedResources()
    {
        LibSceNative.libsce_self_destroy(_libSce, _handle);
        NativeMemory.Free(_dataPtr);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Self()
    {
        ReleaseUnmanagedResources();
    }
}
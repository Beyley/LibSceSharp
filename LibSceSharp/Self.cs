using System.Runtime.InteropServices;

namespace LibSceSharp;

public unsafe class Self : IDisposable
{
    private readonly void* _handle;
    private readonly LibSce _libSce;
    
    private readonly byte* _dataPtr;
    private readonly nuint _dataLen;

    public SelfLoadStatus LoadStatus => LibSceNative.libsce_self_get_load_status(_handle);
    
    public bool NeedsNpdrmLicense => LibSceNative.libsce_self_needs_npdrm_license(_handle);

    public string? ContentId
    {
        get
        {
            const int contentIdSize = 0x30;

            var contentId = stackalloc byte[contentIdSize + 1];
            contentId[contentIdSize] = 0;

            return LibSceNative.libsce_self_get_content_id(_handle, contentId)
                ? Marshal.PtrToStringAnsi((IntPtr)contentId)
                : null;
        }
    }

    // public byte[]? Sha1Hash
    // {
    //     get
    //     {
    //         const int sha1Size = 0x14;
    //
    //         var sha1 = stackalloc byte[sha1Size];
    //
    //         return LibSceNative.libsce_self_get_digest(_handle, sha1) 
    //             ? new Span<byte>(sha1, sha1Size).ToArray() 
    //             : null;
    //     }
    // }

    public Self(LibSce libsce, byte[] data, bool headerOnly = false) : this(libsce, data, null, null, null, null, headerOnly) {}
    public Self(LibSce libsce, byte[] data, byte[] rap) : this(libsce, data, rap, null) {}
    public Self(LibSce libsce, byte[] data, byte[] rif, byte[] actDat, byte[] idps) : this(libsce, data, null, rif, actDat, idps) {}

    private Self(LibSce libsce, byte[] data, byte[]? rap = null, byte[]? rif = null, byte[]? actDat = null,
        byte[]? idps = null, bool headerOnly = false)
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
            if (rap != null)
            {
                if (rap.Length != 0x10)
                    throw new ArgumentOutOfRangeException(nameof(rap), rap.Length.ToString(),
                        "Wanted RAP buffer with length 0x10");

                fixed (byte* rapPtr = rap)
                {
                    // Load the SELF file with a RAP license
                    LibSceNative.HandleError(
                        LibSceNative.libsce_self_load_rap(libsce, _dataPtr, _dataLen, rapPtr, out _handle),
                        nameof(LibSceNative.libsce_self_load_rap));
                }
            }
            else if (rif != null || actDat != null || idps != null)
            {
                ArgumentNullException.ThrowIfNull(rif);
                ArgumentNullException.ThrowIfNull(actDat);
                ArgumentNullException.ThrowIfNull(idps);

                if (idps.Length != 0x10)
                    throw new ArgumentOutOfRangeException(nameof(idps), idps.Length.ToString(),
                        "Wanted IDPS buffer with length 0x10");

                fixed (byte* rifPtr = rif)
                fixed (byte* actDatPtr = actDat)
                fixed (byte* idpsPtr = idps)
                {
                    // Load the SELF file with a RIF license
                    LibSceNative.HandleError(
                        LibSceNative.libsce_self_load_rif(
                            libsce,
                            _dataPtr, _dataLen,
                            rifPtr, (nuint)rif.LongLength,
                            actDatPtr, (nuint)actDat.LongLength,
                            idpsPtr,
                            out _handle),
                        nameof(LibSceNative.libsce_self_load_rif));
                }
            }
            else
            {

                // Load the SELF file
                LibSceNative.HandleError(
                    LibSceNative.libsce_self_load(libsce, _dataPtr, _dataLen, headerOnly, out _handle),
                    nameof(LibSceNative.libsce_self_load));
            }
        }
        catch
        {
            // If we fail, free the memory and exit out
            NativeMemory.Free(_dataPtr);
            throw;
        }
    }

    public Span<byte> ExtractToElf()
    {
        LibSceNative.HandleError(
            LibSceNative.libsce_self_extract_to_elf(_libSce, _handle, out byte* data, out nuint len),
            nameof(LibSceNative.libsce_self_extract_to_elf));

        return new Span<byte>(data, (int)len);
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
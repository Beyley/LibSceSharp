using System.Runtime.InteropServices;

namespace LibSceSharp;

public static unsafe partial class LibSceNative
{
    private const string LibraryName = "sce";

    public const int NoError = -1;
    public const int NoContentIdError = -2;

    [LibraryImport(LibraryName)]
    public static unsafe partial int libsce_create(out void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial int libsce_destroy(void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial void libsce_set_log_callback(delegate* unmanaged[Cdecl]<byte*, LibSceLogLevel, byte*, void> callback);

    [LibraryImport(LibraryName)]
    private static unsafe partial byte* libsce_error_name(int err);

    [LibraryImport(LibraryName)]
    public static unsafe partial int libsce_self_load(void* libsce, byte* cfDataPtr, nuint cfDataLen, out void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial void libsce_self_destroy(void* libsce, void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial SelfLoadStatus libsce_self_get_load_status(void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial Bool32 libsce_self_is_npdrm_application(void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial Bool32 libsce_self_get_content_id(void* handle, byte* contentIdPtr);
    
    public static void HandleError(int err, string func)
    {
        if (err <= NoError) return;

        var errName = Marshal.PtrToStringUTF8((IntPtr)libsce_error_name(err))!;

        throw new LibSceException(errName, func);
    }
}
using System.Runtime.InteropServices;
using System.Text;

namespace LibSceSharp;

public static unsafe partial class Abi
{
    private const string LibraryName = "sce";

    public const int NoError = -1;
    public const int NoContentIdError = -2;

    [LibraryImport(LibraryName)]
    public static unsafe partial int libsce_create(out void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial int libsce_destroy(void* handle);

    [LibraryImport(LibraryName)]
    public static unsafe partial int libsce_get_content_id(void* handle, byte* cfDataPtr, nuint cfDataLen, byte* outPtr);
    
    [LibraryImport(LibraryName)]
    private static unsafe partial byte* libsce_error_name(int err);
    
    public static void HandleError(int err, string func)
    {
        if (err <= NoError) return;

        var errName = Marshal.PtrToStringUTF8((IntPtr)libsce_error_name(err))!;

        throw new LibSceException(errName, func);
    }
}
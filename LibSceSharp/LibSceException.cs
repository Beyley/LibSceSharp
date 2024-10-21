namespace LibSceSharp;

public class LibSceException(string err, string func) : Exception($"Got libsce error {err} from {func}")
{
    public string Error = err;
}
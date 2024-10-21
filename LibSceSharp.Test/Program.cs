namespace LibSceSharp.Test;

class Program
{
    static void Main(string[] args)
    {
        using var libsce = new LibSce();

        var cfData = File.ReadAllBytes(args[0]);
        
        var contentId = libsce.GetContentId(cfData);
        
        Console.WriteLine($"Content ID: \"{contentId}\"");
    }
}
namespace LibSceSharp;

public enum SelfLoadStatus : uint
{
    Full = 0,
    Fake = 1,
    HeaderOnly = 2,
    MissingSystemKey = 3,
    MissingNpdrmKey = 4,
}
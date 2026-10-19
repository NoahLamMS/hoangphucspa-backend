namespace HoangPhucSpa.Data.Entities;

public enum PostKind : byte
{
    News = 1,
    Library = 2
}

public enum RedirectSource : byte
{
    Legacy = 1,
    SlugChange = 2,
    Manual = 3
}

public enum MediaSource : byte
{
    Legacy = 1,
    Upload = 2,
    Demo = 3
}

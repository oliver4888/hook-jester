namespace HookJester.Services.Files
{
    public interface IFileHasher
    {
        byte[] ComputeHash(string filePath);
    }
}

namespace AdverstingPlatforms.Interfaces
{
    public interface IAdverstingPlatformsService
    {
        Task<Dictionary<string, List<string>>> UpdatePlatformsAsync();
        HashSet<string> FindPlatforms(string path);
    }
}

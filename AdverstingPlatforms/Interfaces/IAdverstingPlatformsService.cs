namespace AdverstingPlatforms.Interfaces
{
    public interface IAdverstingPlatformsService
    {
        Task<Dictionary<string, HashSet<string>>> UpdatePlatformsAsync();
        (HashSet<string>, Exception? ex) FindPlatforms(string path);
    }
}

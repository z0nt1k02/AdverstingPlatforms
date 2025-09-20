namespace AdverstingPlatforms.Interfaces
{
    public interface IAdverstingPlatformsService
    {
        Task<Dictionary<string, List<string>>> UpdatePlatforms();
        List<string> FindPlatforms(string path);
    }
}

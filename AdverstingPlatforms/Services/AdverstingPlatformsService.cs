using AdverstingPlatforms.Interfaces;
using System.Buffers;
using System.Text;

namespace AdverstingPlatforms.Services
{
    public class AdverstingPlatformsService : IAdverstingPlatformsService
    {
        private readonly IConfiguration _configuration;
        private Dictionary<string, List<string>> AdverstingPlatforms = new Dictionary<string, List<string>>();
        
        public AdverstingPlatformsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        

        public async Task<Dictionary<string,List<string>>> UpdatePlatforms()
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(_configuration["FilePath"]!.ToString()!, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var s = await reader.ReadLineAsync()!;
                    lines.Add(s!);
                    int colonIndex = s!.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        string key = s.Substring(0, colonIndex);
                        string value = s.Substring(colonIndex + 1);
                        List<string> values = value.Split(',')
                                    .Select(v => v.Trim())
                                    .Where(v => !string.IsNullOrEmpty(v))
                                    .ToList();

                        AdverstingPlatforms[key] = values;
                    }
                }
            }
                     
            return AdverstingPlatforms!;
        }
        public Dictionary<string,List<string>> GetPlatforms()
        {
            return AdverstingPlatforms;
        }

        public List<string> FindPlatforms(string path)
        {
            List<string> result = new List<string>();
            string[] pathParts = path.Split('/');
            string currentPath = "";
            for (int i = 1; i < pathParts.Length; i++)
            {
                currentPath += "/" + pathParts[i];
                
                var platforms =  AdverstingPlatforms
                        .Where(kvp => kvp.Value.Contains(currentPath))
                        .Select(kvp => kvp.Key)
                        .ToList();
                foreach(var platform in platforms)
                {
                    result.Add(platform);
                }
            }
            return result;
        }
        
    }
}

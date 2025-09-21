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

        public async Task<Dictionary<string,List<string>>> UpdatePlatformsAsync()
        {
            List<string> lines = new List<string>();
            try
            {
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
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
                                
            return AdverstingPlatforms!;
        }    

        public HashSet<string> FindPlatforms(string path)
        {
            if (AdverstingPlatforms == null || AdverstingPlatforms.Count == 0)           
                throw new InvalidOperationException("Данные о платформах отсутствуют.Пожалуйста загрузите данные");
            
            if (string.IsNullOrEmpty(path))           
                throw new ArgumentException("Путь не может быть пустым");

            HashSet<string> result = FindPlatformsByPath(path);
                        
            if(result.Count == 0)
                throw new ArgumentNullException("Платформы по текущему запросу ну найдены");
            
            return result;
        } 

        private HashSet<string> FindPlatformsByPath(string path)
        {
            HashSet<string> result = new HashSet<string>();
            string[] pathParts = path.Split('/');
            string currentPath = "";

            for (int i = 1; i < pathParts.Length; i++)
            {
                currentPath += "/" + pathParts[i];
                List<string> platforms = AdverstingPlatforms
                        .AsParallel()
                        .Where(kvp => kvp.Value.Contains(currentPath))
                        .Select(kvp => kvp.Key)
                        .ToList();
                foreach (string platform in platforms)
                {
                    result.Add(platform);
                }
            }
            return result;
        }
        
       
    }
}

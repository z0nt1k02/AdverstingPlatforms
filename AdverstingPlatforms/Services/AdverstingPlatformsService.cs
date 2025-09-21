using AdverstingPlatforms.Interfaces;
using System.Buffers;
using System.Text;

namespace AdverstingPlatforms.Services
{
    public class AdverstingPlatformsService : IAdverstingPlatformsService
    {
        private readonly IConfiguration _configuration;
        private Dictionary<string, HashSet<string>> AdverstingPlatforms = new Dictionary<string, HashSet<string>>();
        
        public AdverstingPlatformsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Dictionary<string,HashSet<string>>> UpdatePlatformsAsync()
        {
            AdverstingPlatforms.Clear();
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
                            HashSet<string> values = value.Split(',')
                                        .Select(v => v.Trim())
                                        .Where(v => !string.IsNullOrEmpty(v))
                                        .ToHashSet();

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

        public (HashSet<string>,Exception? ex) FindPlatforms(string path)
        {
            if (AdverstingPlatforms == null || AdverstingPlatforms.Count == 0)           
                throw new InvalidOperationException("Данные о платформах отсутствуют.Пожалуйста загрузите данные");
            
            if (string.IsNullOrEmpty(path))           
                throw new ArgumentException("Путь не может быть пустым");

            var (result,error) = FindPlatformsByPath(path);
            
                
                        
            if(result.Count == 0)
                throw new ArgumentNullException("Платформы по текущему запросу ну найдены");
            
            return (result, error);
        } 

        private (HashSet<string>,Exception? ex) FindPlatformsByPath(string path)
        {
            HashSet<string> result = new HashSet<string>();
            string[] pathParts = path.Split('/');
            string currentPath = "";
            bool exists = AdverstingPlatforms.Values.Any(set => set.Contains(path));
            
            for (int i = 1; i < pathParts.Length; i++)
            {
                currentPath += "/" + pathParts[i];
                List<string> platforms = AdverstingPlatforms
                        .AsParallel()
                        .Where(kvp => kvp.Value.Contains(currentPath))
                        .Select(kvp => kvp.Key)
                        .ToList();
                if (platforms.Count == 0)
                {
                    return (result, new Exception($"Регион {pathParts[i]} не найден.Показаны ближайшие результаты"));                   
                }
                foreach (string platform in platforms)
                {
                    result.Add(platform);
                }
            }
            return (result,null);
        }
        
       
    }
}

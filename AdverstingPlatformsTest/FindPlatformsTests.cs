namespace AdverstingPlatformsTest
{
    public class FindPlatformsTests
    {
        private class TestService
        {
            public Dictionary<string, HashSet<string>> AdverstingPlatforms = new Dictionary<string, HashSet<string>>           
            {
                { "Яндекс.Директ", new HashSet<string> { "/ru" } },
                { "Ревдинский рабочий", new HashSet<string> { "/ru/svrd/revda", "/ru/svrd/pervik" } },
                { "Газета уральских москвичей", new HashSet<string>{"/ru/msk","ru/permobl","/ru/chelobl"} },
                { "Крутая реклама", new HashSet<string> { "/ru/svrd" } },
                { "Московские известия", new HashSet<string>{"/ru/msk","/ru/msk/chertanovo","/ru/msk/ramensky"} }
            };

            public (HashSet<string>, Exception? ex) FindPlatformsByPath(string path)
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
                return (result, null);
            }
        }
        

        [Fact]
        public void FindPlatform_Succesfull_Test()
        {
            TestService service = new TestService();           

            Dictionary<string, List<string>> correctAnswers = new Dictionary<string, List<string>>
            {
                {"/ru/svrd/revda",new List<string>{ "Яндекс.Директ", "Крутая реклама", "Ревдинский рабочий" } },
                {"/ru/msk/chertanovo", new List<string>{ "Яндекс.Директ", "Газета уральских москвичей", "Московские известия" }}
            };
            foreach(var answer in correctAnswers)
            {
                var (platforms, exception) = service.FindPlatformsByPath(answer.Key);
                Assert.Null(exception);
                Assert.Equal(platforms.ToList().Count, answer.Value.Count);
                
                for(int i = 0; i < answer.Value.Count;i++)
                {
                    Assert.Contains(answer.Value[i], platforms);
                }         
            }
        }

        [Fact]
        public void ErroneousRegions_Exception_Test()
        {
            TestService service = new TestService();

            Dictionary<string, string> correctAnswers = new Dictionary<string, string>
            {
                {"/ru/A","Регион A не найден.Показаны ближайшие результаты" },
                {"/ru/B","Регион B не найден.Показаны ближайшие результаты" },
                {"/ru/C","Регион C не найден.Показаны ближайшие результаты" },
                {"/ru/D","Регион D не найден.Показаны ближайшие результаты" }

            };
            foreach(var answer in correctAnswers)
            {
                var (platforms, exception) = service.FindPlatformsByPath(answer.Key);
                Assert.NotNull(exception);
                Assert.Equal(answer.Value, exception!.Message);
            }
        }
    }
}

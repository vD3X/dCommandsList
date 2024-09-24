using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static dCommandsList.dCommandsList;

namespace dCommandsList
{
    public static class Config
    {
        private static readonly string configPath = Path.Combine(Instance.ModuleDirectory, "Config.json");
        public static ConfigModel config;
        private static FileSystemWatcher fileWatcher;

        public static ConfigModel LoadedConfig => config;

        public static void Initialize()
        {
            config = LoadConfig();
            SetupFileWatcher();
        }

        private static ConfigModel LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                Instance.Logger.LogInformation("Plik konfiguracyjny nie istnieje. Tworzenie nowego pliku konfiguracyjnego...");
                var defaultConfig = new ConfigModel();
                SaveConfig(defaultConfig);
                return defaultConfig;
            }

            try
            {
                string json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<ConfigModel>(json) ?? new ConfigModel();
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Błąd podczas wczytywania pliku konfiguracyjnego.");
                return new ConfigModel();
            }
        }

        public static void SaveConfig(ConfigModel config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError($"Błąd podczas zapisywania pliku konfiguracyjnego: {ex.Message}");
            }
        }

        private static void SetupFileWatcher()
        {
            fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(configPath))
            {
                Filter = Path.GetFileName(configPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            fileWatcher.Changed += (sender, e) => config = LoadConfig();
            fileWatcher.EnableRaisingEvents = true;
        }

        public class ConfigModel
        {
            public Settings Settings { get; set; } = new Settings();
        }

        public class Settings
        {
            public string DisplayMode { get; set; } = "both";
            public string Help_Commands { get; set; } = "pomoc, help, komendy, commands";
            public string Menu_Title { get; set; } = "[ ★ CS-Zjarani | Pomoc ★ ]";
            public string Menu_Title_Color { get; set; } = "#29cc94";
            public bool Menu_Close_After_Select { get; set; } = true;
        }
    }
}
using Newtonsoft.Json;
using System.Diagnostics;

namespace MaisonConnecteBlazor.Configuration
{
    public static class ConfigManager
    {
        public static Config CurrentConfig { get; set; } = new Config();

        public static void Initialize()
        {
            if (!File.Exists("config.json"))
            {
                Debug.WriteLine("Fichier de configuration créé");
                File.WriteAllText("config.json", JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented));
            }
            else
            {
                Debug.WriteLine("Fichier de configuration chargé");
                CurrentConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
        }
    }
}

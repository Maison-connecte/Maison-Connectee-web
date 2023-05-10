using Newtonsoft.Json;
using System.Diagnostics;

namespace MaisonConnecteBlazor.Configuration
{
    /// <summary>
    /// Classe s'occupant de gérer la configuration de l'appl,ication
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// Config, Config présentement chargée
        /// </summary>
        public static Config ConfigurationPresente { get; set; } = new Config();

        /// <summary>
        /// Fonction qui sert à initialiser le manager de configuration
        /// </summary>
        public static void Initialize()
        {
            // On valide que la configuration existe, sinon on en crée une par défaut
            if (!File.Exists("config.json"))
            {
                Debug.WriteLine("Fichier de configuration créé");
                File.WriteAllText("config.json", JsonConvert.SerializeObject(ConfigurationPresente, Formatting.Indented));
            }
            else // Sinon on charge la configuration existante
            {
                Debug.WriteLine("Fichier de configuration chargé");
                ConfigurationPresente = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
        }
    }
}

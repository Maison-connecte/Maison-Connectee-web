using MaisonConnecteBlazor.Components.Base;
using MQTTnet;
using MQTTnet.Client;
using MudBlazor.Utilities;

namespace MaisonConnecteBlazor.Pages
{
    /// <summary>
    /// Classe gérant la page de gestion du matériel
    /// </summary>
    public partial class Management : MaisonConnecteBase
    {
        // Information de connexion et des topics MQTT
        public const string Serveur = "test.mosquitto.org";
        public const int PortServeur = 1883;
        public const string SujetCouleur = "couleur_led_divertissement";
        public const string SujetAllumeLED = "allumer_led_divertissement";

        // Initialiser des variables
        public bool LEDAllume { get; set; } = true;
        public MudColor Couleur { get; set; } = "#FF0000FF";

        /// <summary>
        /// Méthode envoyant les données de la page à MQTT
        /// </summary>
        public async Task SendMQTTData()
        {
            // Formattage de l'information à envoyer
            float Intensite = (Couleur.A / 255f);
            int R = LEDAllume ? (int)(Couleur.R * Intensite) : 0;
            int G = LEDAllume ? (int)(Couleur.G * Intensite) : 0;
            int B = LEDAllume ? (int)(Couleur.B * Intensite) : 0;
            string DonneCouleur = string.Join("/", new List<string>() { R.ToString(), G.ToString(), B.ToString()});

            // Création de la connexion avec le serveur
            MqttClientOptionsBuilder constructeur = new MqttClientOptionsBuilder();
            constructeur.WithClientId(Guid.NewGuid().ToString());
            constructeur.WithTcpServer(Serveur, PortServeur);

            // Création du client MQTT
            MqttFactory usine = new MqttFactory();
            IMqttClient client = usine.CreateMqttClient();

            // Connexion au serveur
            await client.ConnectAsync(constructeur.Build());

            // Création du message pour la couleur
            MqttApplicationMessageBuilder ConstructeurCouleur = new MqttApplicationMessageBuilder();
            ConstructeurCouleur.WithTopic(SujetCouleur);
            ConstructeurCouleur.WithPayload(DonneCouleur);

            // Création du message pour activer/désactiver la LED
            MqttApplicationMessageBuilder AllumeConstructeur = new MqttApplicationMessageBuilder();
            AllumeConstructeur.WithTopic(SujetAllumeLED);
            AllumeConstructeur.WithPayload(LEDAllume ? "1" : "0");

            // Envoie des messages
            await client.PublishAsync(ConstructeurCouleur.Build());
            await client.PublishAsync(AllumeConstructeur.Build());

            // Déconnexion du client et montrer un message à l'utilisateur
            await client.DisconnectAsync();
            Snackbar.Add("Les informations ont bien été envoyés par MQTT!", MudBlazor.Severity.Success);
        }
    }
}

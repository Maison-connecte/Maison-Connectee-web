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
        public const string Server = "test.mosquitto.org";
        public const int ServerPort = 1883;
        public const string ColorTopic = "colordylan";
        public const string LEDEnableTopic = "enable";

        // Initialiser des variables
        public bool LEDAllume { get; set; } = true;
        public MudColor Color { get; set; } = "#FF0000FF";

        /// <summary>
        /// Méthode envoyant les données de la page à MQTT
        /// </summary>
        public async Task SendMQTTData()
        {
            // Formattage de l'information à envoyer
            float Intensite = (Color.A / 255f);
            int R = LEDAllume ? (int)(Color.R * Intensite) : 0;
            int G = LEDAllume ? (int)(Color.G * Intensite) : 0;
            int B = LEDAllume ? (int)(Color.B * Intensite) : 0;
            string ColorData = string.Join("/", new List<string>() { R.ToString(), G.ToString(), B.ToString(), LEDAllume ? "1" : "0"});

            // Création de la connexion avec le serveur
            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder();
            builder.WithClientId(Guid.NewGuid().ToString());
            builder.WithTcpServer(Server, ServerPort);

            // Création du client MQTT
            MqttFactory factory = new MqttFactory();
            IMqttClient client = factory.CreateMqttClient();

            // Connexion au serveur
            await client.ConnectAsync(builder.Build());

            // Création du message pour la couleur
            MqttApplicationMessageBuilder ColorBuilder = new MqttApplicationMessageBuilder();
            ColorBuilder.WithTopic(ColorTopic);
            ColorBuilder.WithPayload(ColorData);

            // Création du message pour activer/désactiver la LED
            MqttApplicationMessageBuilder EnabledBuilder = new MqttApplicationMessageBuilder();
            EnabledBuilder.WithTopic(LEDEnableTopic);
            EnabledBuilder.WithPayload(LEDAllume ? "1" : "0");

            // Envoie des messages
            await client.PublishAsync(ColorBuilder.Build());
            await client.PublishAsync(EnabledBuilder.Build());

            // Déconnexion du client et montrer un message à l'utilisateur
            await client.DisconnectAsync();
            Snackbar.Add("Les informations ont bien été envoyés par MQTT!", MudBlazor.Severity.Success);
        }
    }
}

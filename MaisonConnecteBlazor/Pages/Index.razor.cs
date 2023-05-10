using Microsoft.VisualStudio.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Configuration;

namespace MaisonConnecteBlazor.Pages
{
    /// <summary>
    /// Classe gérant l'index de la page, la page avec le flux vidéo en direct de la caméra
    /// </summary>
    public partial class Index : MaisonConnecteBase, IDisposable
    {
        // Initialisation des variables
        private Socket? socketClient;
        public string Image64 { get; set; } = string.Empty;

        /// <summary>
        /// Méthode appelé au moment de l'initialisation de la page
        /// </summary>
        protected override Task OnInitializedAsync()
        {
            // Connexion au socket
            Task.Run(SetupSocketConnection).Forget();

            return base.OnInitializedAsync();
        }

        public async Task SetupSocketConnection()
        {
            // On valide que le socket n'existe pas
            Dispose();

            // On crée la connexion au socket
            string IPServeur = ConfigManager.ConfigurationPresente.IPFluxVideo;
            int port = ConfigManager.ConfigurationPresente.PortFluxVideo;

            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(IPServeur), port);

            try
            {
                // On se connecte au socket et on reçoit des données indéfiniement
                await socketClient.ConnectAsync(serverEndPoint);

                byte[]? buffer = new byte[100000000];
                int bytesReceived;

                while ((bytesReceived = socketClient.Receive(buffer)) > 0)
                {
                    Image64 = string.Concat("data:image/jpeg;base64,", Encoding.ASCII.GetString(buffer, 0, bytesReceived)).Replace("---END_OF_FRAME---", "");
                    UpdateImage();
                }
                buffer = null;
            }
            catch (SocketException ex) // On attrape les erreurs
            {
                Debug.WriteLine("Exception du socket:" + ex.Message);
            }
            catch(ObjectDisposedException)
            {
                Debug.WriteLine("Socket tué");
            }
            catch(NullReferenceException) { }
            finally
            {
                // Lorsque c'est fini, on dispose du socket
                Dispose();
            }
        }
          
        /// <summary>
        /// Méthode qui sert à mettre l'image à jour dans le navigateur
        /// </summary>
        public async void UpdateImage()
        {
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Méthode qui sert à tuer le socket
        /// </summary>
        public void Dispose()
        {
            if (socketClient != null)
            {
                socketClient.Close();
                socketClient.Dispose();
                socketClient = null;

                GC.Collect();
            }
        }
    }
}

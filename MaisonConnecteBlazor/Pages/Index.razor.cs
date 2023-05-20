using Microsoft.VisualStudio.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Configuration;
using System.Drawing;

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
                string imageRecu;

                while ((bytesReceived = socketClient.Receive(buffer)) > 0)
                {
                    imageRecu = Encoding.ASCII.GetString(buffer, 0, bytesReceived).Replace("---END_OF_FRAME---", "");
                    if (ImageBase64Valide(imageRecu))
                    {
                        Image64 = string.Concat("data:image/jpeg;base64,", imageRecu);
                        UpdateImage();
                    }
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

        private bool ImageBase64Valide(string image64)
        {
            bool imageValid = false;
            byte[]? bytesImage = null;
            MemoryStream? stream = null;
            Image? imageValidation = null;
            try
            {
                bytesImage = Convert.FromBase64String(image64);
                stream = new MemoryStream(bytesImage);

                imageValidation = Image.FromStream(stream);
                

                imageValid = true;
            } catch
            {
                imageValid = false;
            } finally
            {
                if (imageValidation != null)
                {
                    imageValidation.Dispose();
                    imageValidation = null;
                }

                if(bytesImage != null)
                    bytesImage = null;

                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }

            return imageValid;
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

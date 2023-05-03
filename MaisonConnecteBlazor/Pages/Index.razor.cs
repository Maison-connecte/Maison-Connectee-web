using Microsoft.VisualStudio.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Configuration;

namespace MaisonConnecteBlazor.Pages
{
    public partial class Index : MaisonConnecteBase, IDisposable
    {
        private Socket clientSocket;
        public string Image64 { get; set; } = string.Empty;

        protected override Task OnInitializedAsync()
        {
            Task.Run(SetupSocketConnection).Forget();

            return base.OnInitializedAsync();
        }

        public Task SetupSocketConnection()
        {
            Dispose();

            string serverIpAddress = ConfigManager.CurrentConfig.VideoFeedIP;
            int port = ConfigManager.CurrentConfig.VideoFeedPort;

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), port);

            try
            {
                clientSocket.Connect(serverEndPoint);
                Debug.WriteLine("Connected to the server.");

                // Receive data from the server
                byte[] buffer = new byte[100000000]; // Buffer size can be adjusted as needed
                int bytesReceived;

                while ((bytesReceived = clientSocket.Receive(buffer)) > 0)
                {
                    Image64 = string.Concat("data:image/jpeg;base64,", Encoding.ASCII.GetString(buffer, 0, bytesReceived)).Replace("---END_OF_FRAME---", "");
                    UpdateImage();
                }
                buffer = null;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"Socket exception: {ex.Message}");
            }
            catch(ObjectDisposedException ex)
            {
                Debug.WriteLine("Socket killed");
            }
            catch(NullReferenceException ex) { }
            finally
            {
                Dispose();
                Debug.WriteLine("Connection closed.");
            }

            //socketClient.Connect("localhost", 8010);
            //socketClient.StringEncoder = System.Text.Encoding.ASCII;
            //RequestImage();
            //socketClient.DataReceived += (sender, msg) =>
            //{
            //    //Debug.WriteLine(msg.MessageString);
            //    Debug.WriteLine(msg.MessageString);
            //    Image64 = "data:image/jpg; base64, " + msg.MessageString.Substring(0, msg.MessageString.Length - 1);
            //    UpdateImage();
            //};



            return Task.CompletedTask;
        }
          
        public async void UpdateImage()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket.Dispose();
                clientSocket = null;

                GC.Collect();
            }

            Debug.WriteLine("Disposed of socket");
        }
    }
}

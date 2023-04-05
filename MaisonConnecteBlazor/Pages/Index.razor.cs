using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Threading;
using SimpleTCP;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace MaisonConnecteBlazor.Pages
{
    public partial class Index : ComponentBase
    {
        private SimpleTcpClient socketClient = new SimpleTcpClient();
        public string Image64 { get; set; } = string.Empty;

        protected override Task OnInitializedAsync()
        {
            Task.Run(SetupSocketConnection).Forget();

            return base.OnInitializedAsync();
        }

        public Task SetupSocketConnection()
        {
            int port = 8010;
            string serverIpAddress = "10.10.211.27"; // Replace with your server's IP address

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                    string test2 = Convert.ToBase64String(buffer, 0, bytesReceived);
                    string test = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                    Image64 = string.Concat("data:image/jpeg;base64,", test);
                    Debug.WriteLine(test.Length / 1024);
                    if(!File.Exists("image8.txt"))
                    {
                        File.WriteAllText("image8.txt", test);
                        //UpdateImage();
                    }
                    UpdateImage();
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"Socket exception: {ex.Message}");
            }
            finally
            {
                clientSocket.Close();
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

        private void RequestImage()
        {
            socketClient.WriteLine("Need_Image_PLS");
        }
          
        public async void UpdateImage()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}

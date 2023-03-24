using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Threading;
using SimpleTCP;
using System.Diagnostics;

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
            socketClient.Connect("10.10.211.27", 8010);
            socketClient.StringEncoder = System.Text.Encoding.ASCII;
            socketClient.DataReceived += (sender, msg) =>
            {
                Debug.WriteLine(msg.MessageString);
                Image64 = "data:image/png; base64, " + msg.MessageString;
                UpdateImage();
            };

            return Task.CompletedTask;
        }
          
        public async void UpdateImage()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}

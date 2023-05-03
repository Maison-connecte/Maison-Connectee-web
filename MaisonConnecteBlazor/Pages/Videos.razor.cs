using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Database;
using MaisonConnecteBlazor.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace MaisonConnecteBlazor.Pages
{
    public class EnregistrementPreview
    {
        public byte[]? Thumbnail;
        public DateTime Temps;
    }
    public partial class Videos : MaisonConnecteBase
    {
        List<EnregistrementPreview> videos = new List<EnregistrementPreview>();

        protected override async Task OnInitializedAsync()
        {
            await GetVideos();
            await base.OnInitializedAsync();
        }

        private async Task GetVideos()
        {
            DBConnect context = new DBConnect();

            videos = await context.db.Enregistrements.Select(enregistrement => new EnregistrementPreview()
            {
                Thumbnail = enregistrement.Thumbnail,
                Temps = enregistrement.Date,
            }).ToListAsync();

            await InvokeAsync(StateHasChanged);
        }
    }
}

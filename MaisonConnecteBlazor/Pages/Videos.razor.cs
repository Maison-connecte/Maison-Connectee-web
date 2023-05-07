using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Database;
using Microsoft.EntityFrameworkCore;

namespace MaisonConnecteBlazor.Pages
{
    /// <summary>
    /// Classe gérant les enregistrement sans avoir besoin de télécharger la vidéo aussi
    /// </summary>
    public class EnregistrementPreview
    {
        public long ID;
        public byte[]? Thumbnail;
        public DateTime Temps;
    }

    /// <summary>
    /// Classe gérant la page de galerie de vidéo
    /// </summary>
    public partial class Videos : MaisonConnecteBase
    {
        // Initialisation des variables
        List<EnregistrementPreview> videos = new List<EnregistrementPreview>();

        /// <summary>
        /// Méthode qui est éxécuter au moment de l'initialisation de la page
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            // On montre un chargement et on obtient les vidéos
            SpinnerService.Show();
            await GetVideos();
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Méthode qui obtient les vidéos à partir de la base de données
        /// </summary>
        private async Task GetVideos()
        {
            // On se connecte à la BD et on obtient les vidéos
            DBConnect context = new DBConnect();
            videos = await context.db.Enregistrements.AsNoTracking().Select(enregistrement => new EnregistrementPreview()
            {
                ID = enregistrement.Id,
                Thumbnail = enregistrement.Thumbnail,
                Temps = enregistrement.Date,
            }).ToListAsync();

            // On met à jour la page et on cache le chargement
            await InvokeAsync(StateHasChanged);
            await InvokeAsync(SpinnerService.Hide);
        }
    }
}

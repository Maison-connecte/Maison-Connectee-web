using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Database;
using Microsoft.EntityFrameworkCore;

namespace MaisonConnecteBlazor.Pages
{
    /// <summary>
    /// Classe gérant les enregistrement sans avoir besoin de télécharger la vidéo aussi
    /// </summary>
    public class PrevisualisationVideo
    {
        public long ID;
        public byte[]? Apercu;
        public DateTime Temps;
    }

    /// <summary>
    /// Classe gérant la page de galerie de vidéo
    /// </summary>
    public partial class Videos : MaisonConnecteBase
    {
        // Initialisation des variables
        List<PrevisualisationVideo> videos = new List<PrevisualisationVideo>();

        /// <summary>
        /// Méthode qui est éxécuter au moment de l'initialisation de la page
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            // On montre un chargement et on obtient les vidéos
            SpinnerService.Show();
            await ObtenirVideos();
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Méthode qui obtient les vidéos à partir de la base de données
        /// </summary>
        private async Task ObtenirVideos()
        {
            // On se connecte à la BD et on obtient les vidéos
            DBConnect context = new DBConnect();
            videos = await context.db.Enregistrements.AsNoTracking().Select(enregistrement => new PrevisualisationVideo()
            {
                ID = enregistrement.Id,
                Apercu = enregistrement.Thumbnail,
                Temps = enregistrement.Date,
            }).ToListAsync();

            // On met à jour la page et on cache le chargement
            await InvokeAsync(StateHasChanged);
            await InvokeAsync(SpinnerService.Hide);
        }
    }
}

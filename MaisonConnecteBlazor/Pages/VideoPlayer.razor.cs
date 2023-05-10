using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Database;
using MaisonConnecteBlazor.Database.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace MaisonConnecteBlazor.Pages
{
    /// <summary>
    /// Classe qui gère la page de lecture de vidéos
    /// </summary>
    public partial class VideoPlayer : MaisonConnecteBase
    {
        // Initialisation des variables
        [Parameter]
        public int? ID { get; set; }
        private byte[]? FluxVideo;

        /// <summary>
        /// Méthode qui s'éxecute au moment de l'initialisation de la page
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            // On montre le chargement
            SpinnerService.Show();

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Méthode qui s'éxecute quand la page à fini de s'éxécuter
        /// </summary>
        /// <param name="firstRender">bool, Si c'est la première fois que la page est rendu</param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // On obtient la vidéo
            await ObtenirVideo();

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Méthode qui obtient la vidéo et la fait jouer sur la page
        /// </summary>
        public async Task ObtenirVideo()
        {
            // Obtention de la vidéo
            DBConnect context = new DBConnect();
            Enregistrement? enregistrement = context.db.Enregistrements.AsNoTracking().SingleOrDefault(enregistrement => enregistrement.Id == ID);

            // Si l'enregistrement est nul on retourne l'utilisateur à la page de galerie
            if (enregistrement == null)
            {
                NavigationManager.NavigateTo("videos");
            } else
            {
                // On montre la vidéo
                FluxVideo = enregistrement.FluxVideo;
                string url = await JSRuntime.InvokeAsync<string>("creeURLVideo", FluxVideo);
                await JSRuntime.InvokeVoidAsync("definirSourceVideo", "lecteurVideo", url);

                // On enlève le chargement
                await InvokeAsync(SpinnerService.Hide);
            }
        }

        /// <summary>
        /// Méthode qui retourne l'utilisateur à la page "Galerie Vidéo"
        /// </summary>
        public void Retour()
        {
            NavigationManager.NavigateTo("videos");
        }
    }
}

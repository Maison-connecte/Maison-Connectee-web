using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Extensions;

namespace MaisonConnecteBlazor.Components.Includes
{
    /// <summary>
    /// Enum pour toutes les pages disponibles
    /// </summary>
    public enum LiensHeader
    {
        ACCUEIL,
        VIDEOS,
        STATS,
        GESTION,
    }

    /// <summary>
    /// Classe gérant le header des pages
    /// </summary>
    public partial class MaisonConnecteHeader : MaisonConnecteBase
    {
        // Identifiant pour chaque page
        public const string VideosIdentifiant = "videos";
        public const string StatsIdentifiant = "stats";
        public const string GestionIdentifiant = "manage";

        // Initialisation des variables
        public Dictionary<LiensHeader, bool> PageActive { get; set; } = new Dictionary<LiensHeader, bool>();
        public bool TiroirOuvert { get; set; } = false;

        protected override void OnParametersSet()
        {
            RegarderPageActive();

            base.OnParametersSet();
        }

        /// <summary>
        /// Méthode qui sert a valider la page où l'utilisateur se trouve présentement
        /// </summary>
        private void RegarderPageActive()
        {
            // Initialisation de la page active
            PageActive = new Dictionary<LiensHeader, bool>();

            foreach (LiensHeader header in Enum.GetValues(typeof(LiensHeader)))
            {
                PageActive[header] = false;
            }

            // Manipulation de strings pour trouver la page que l'utilisateur se trouve présentement
            string url = NavigationManager.Uri.Replace("www", "").Replace("http://", "").Replace("https://", "");
            int indexDuSlash = url.IndexOf("/");

            if (indexDuSlash == -1)
            {
                PageActive[LiensHeader.ACCUEIL] = true;
            }
            else
            {
                int compteur = url.Count(character => character == '/');

                string identifiant;
                if (compteur == 1)
                {
                    identifiant = url.Substring(indexDuSlash + 1);
                }
                else
                {
                    int secondIndex = url.IndexOfNth("/", 2);
                    identifiant = url.Substring(indexDuSlash + 1, secondIndex - 1 - indexDuSlash);
                }

                switch (identifiant)
                {
                    case "":
                        PageActive[LiensHeader.ACCUEIL] = true;
                        break;
                    case VideosIdentifiant:
                        PageActive[LiensHeader.VIDEOS] = true;
                        break;
                    case StatsIdentifiant:
                        PageActive[LiensHeader.STATS] = true;
                        break;
                    case GestionIdentifiant:
                        PageActive[LiensHeader.GESTION] = true;
                        break;
                    default:
                        break;
                }
            }

            StateHasChanged();
        }

        /// <summary>
        /// Méthode qui ouvre/ferme le "drawer" utilisable sur mobile
        /// </summary>
        private void BasculerTiroir()
        {
            TiroirOuvert = !TiroirOuvert;
        }
    }
}

using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Extensions;

namespace MaisonConnecteBlazor.Components.Includes
{
    /// <summary>
    /// Enum pour toutes les pages disponibles
    /// </summary>
    public enum HeaderLinks
    {
        HOME,
        VIDEOS,
        STATS,
        MANAGE,
    }

    /// <summary>
    /// Classe gérant le header des pages
    /// </summary>
    public partial class MaisonConnecteHeader : MaisonConnecteBase
    {
        // Identifiant pour chaque page
        public const string VideosIdentifier = "videos";
        public const string StatsIdentifier = "stats";
        public const string ManageIdentifier = "manage";

        // Initialisation des variables
        public Dictionary<HeaderLinks, bool> ActivePage { get; set; } = new Dictionary<HeaderLinks, bool>();
        public bool DrawerOpened { get; set; } = false;

        protected override void OnParametersSet()
        {
            CheckActivePage();

            base.OnParametersSet();
        }

        /// <summary>
        /// Méthode qui sert a valider la page où l'utilisateur se trouve présentement
        /// </summary>
        private void CheckActivePage()
        {
            // Initialisation de la page active
            ActivePage = new Dictionary<HeaderLinks, bool>();

            foreach (HeaderLinks header in Enum.GetValues(typeof(HeaderLinks)))
            {
                ActivePage[header] = false;
            }

            // Manipulation de strings pour trouver la page que l'utilisateur se trouve présentement
            string url = NavigationManager.Uri.Replace("www", "").Replace("http://", "").Replace("https://", "");
            int indexOfSlash = url.IndexOf("/");

            if (indexOfSlash == -1)
            {
                ActivePage[HeaderLinks.HOME] = true;
            }
            else
            {
                int compteur = url.Count(character => character == '/');

                string identifier;
                if (compteur == 1)
                {
                    identifier = url.Substring(indexOfSlash + 1);
                }
                else
                {
                    int secondIndex = url.IndexOfNth("/", 2);
                    identifier = url.Substring(indexOfSlash + 1, secondIndex - 1 - indexOfSlash);
                }

                switch (identifier)
                {
                    case "":
                        ActivePage[HeaderLinks.HOME] = true;
                        break;
                    case VideosIdentifier:
                        ActivePage[HeaderLinks.VIDEOS] = true;
                        break;
                    case StatsIdentifier:
                        ActivePage[HeaderLinks.STATS] = true;
                        break;
                    case ManageIdentifier:
                        ActivePage[HeaderLinks.MANAGE] = true;
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
        private void ToggleDrawer()
        {
            DrawerOpened = !DrawerOpened;
        }
    }
}

﻿using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Extensions;

namespace MaisonConnecteBlazor.Components.Includes
{
    public enum HeaderLinks
    {
        HOME,
        VIDEOS,
        STATS,
    }

    public partial class MaisonConnecteHeader : MaisonConnecteBase
    {
        public const string VideosIdentifier = "videos";
        public const string StatsIdentifier = "stats";

        public Dictionary<HeaderLinks, bool> ActivePage { get; set; } = new Dictionary<HeaderLinks, bool>();
        public bool DrawerOpened { get; set; } = false;

        protected override void OnParametersSet()
        {
            CheckActivePage();

            base.OnParametersSet();
        }

        private void CheckActivePage()
        {
            ActivePage = new Dictionary<HeaderLinks, bool>();

            foreach (HeaderLinks header in Enum.GetValues(typeof(HeaderLinks)))
            {
                ActivePage[header] = false;
            }

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
                    default:
                        break;
                }
            }

            StateHasChanged();
        }

        private void ToggleDrawer()
        {
            DrawerOpened = !DrawerOpened;
        }
    }
}

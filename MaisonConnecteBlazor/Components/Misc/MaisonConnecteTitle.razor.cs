using MaisonConnecteBlazor.Components.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaisonConnecteBlazor.Components.Misc
{
    /// <summary>
    /// Classe gérant le composant "MaisonConnecteTitle" pour afficher les titres en haut des pages
    /// </summary>
    public partial class MaisonConnecteTitle : MaisonConnecteBase
    {
        // Initialisation des variables
        [Parameter]
        public Typo Typo { get; set; } = Typo.h2;
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}

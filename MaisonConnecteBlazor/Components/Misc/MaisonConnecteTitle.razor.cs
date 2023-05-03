using MaisonConnecteBlazor.Components.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MaisonConnecteBlazor.Components.Misc
{
    public partial class MaisonConnecteTitle : MaisonConnecteBase
    {
        [Parameter]
        public Typo Typo { get; set; } = Typo.h2;
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}

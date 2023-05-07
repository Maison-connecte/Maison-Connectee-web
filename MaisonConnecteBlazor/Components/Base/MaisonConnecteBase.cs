using BlazorSpinner;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaisonConnecteBlazor.Components.Base
{
    /// <summary>
    /// Classe abstraite contenant la base de tous les composants du projet
    /// </summary>
    public abstract partial class MaisonConnecteBase : ComponentBase
    {
        // Injection des composants requis
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public SpinnerService SpinnerService { get; set; } = default!;
    }
}

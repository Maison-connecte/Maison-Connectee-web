using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MaisonConnecteBlazor.Components.Base
{
    public abstract partial class MaisonConnecteBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        public async Task OpenInNewWindow(string url)
        {
            await JSRuntime.InvokeVoidAsync("eval", "let _discard_ = setTimeout(() => {open(`" + url + "`, `_blank`)})");
        }
    }
}

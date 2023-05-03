using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Database.Models;

namespace MaisonConnecteBlazor.Pages
{
    public partial class Videos : MaisonConnecteBase
    {
        List<Enregistrement> videos = new List<Enregistrement>();
        protected override void OnInitialized()
        {
            base.OnInitialized();

            for(int i = 0; i < 10; i++)
            {
                videos.Add(new Enregistrement() { Date = DateTime.Now});
            }
        }
    }
}

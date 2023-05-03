namespace MaisonConnecteBlazor.Database.Models;

public partial class Enregistrement
{
    public long Id { get; set; }

    public byte[] FluxVideo { get; set; } = null!;

    public byte[] Thumbnail { get; set; } = null!;

    public DateTime Date { get; set; }
}

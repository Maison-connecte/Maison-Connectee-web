using System;
using System.Collections.Generic;

namespace MaisonConnecteBlazor.Database.Models;

public partial class Event
{
    public long Id { get; set; }

    public DateTime? Date { get; set; }

    public string? Event1 { get; set; }

    public string? Data { get; set; }
}

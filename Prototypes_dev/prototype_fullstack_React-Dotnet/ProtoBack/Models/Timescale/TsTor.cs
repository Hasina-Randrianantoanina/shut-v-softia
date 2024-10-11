using System;
using System.Collections.Generic;

namespace ProtoBack.Models.Timescale;

public partial class TsTor
{
    public string Libelle { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public bool? Valeur { get; set; }
}

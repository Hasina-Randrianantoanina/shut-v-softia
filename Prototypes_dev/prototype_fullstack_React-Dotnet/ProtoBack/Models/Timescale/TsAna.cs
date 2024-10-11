using System;
using System.Collections.Generic;

namespace ProtoBack.Models.Timescale;

public partial class TsAna
{
    public string Libelle { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public float? Valeur { get; set; }
}

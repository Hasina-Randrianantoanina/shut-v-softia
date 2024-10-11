﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Domain
{
    public class HistoricalDataPoint
    {
        public string Horodate { get; set; }
        public string Evenement { get; set; }
        public string Mesure { get; set; }
        public string MesureBrute { get; set; }
        public string? Voie { get; set; }
    }
    public record HistoricalDataDTO(string Libelle, string DateTime, string Valeur);
}

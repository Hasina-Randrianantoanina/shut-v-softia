﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Domain
{
    public class HistoricalDataPoint
    {
        public string? Horodate { get; set; }
        public string? Evenement { get; set; }
        public string? Mesure { get; set; }
        public string? MesureBrute { get; set; }
        public string? Voie { get; set; }
        public string? Station { get; set; }
    }

    public record HistoricalDataDTO(string Station, string Libelle, string DateTime, string Valeur);

    public record HistoricalDataCsvFormat(List<(string?horodate, string? Evenement)> TimeStamps, List<HistoricalDataVoieCsv> dataVoies);
    public record HistoricalDataVoieCsv(string voie, List<string> Valeurs);

    public class TimedEvent
    {
        public DateTime TimeStamp { get; set; }
        public string Evenement { get; set; } = string.Empty;

        public bool Equals(TimedEvent other)
        {
            return TimeStamp.Equals(other.TimeStamp) && Evenement.Equals(other.Evenement);
        }

    }
}
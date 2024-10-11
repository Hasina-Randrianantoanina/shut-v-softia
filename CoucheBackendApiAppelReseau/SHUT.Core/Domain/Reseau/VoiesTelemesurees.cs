using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SHUT.Core.Domain.Reseau
{
    [Table("voies_telemesurees", Schema = "reseau")]
    public class VoiesTelemesurees
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("adresse_bes")]
        public int? AdresseBes { get; set; }

        [Column("delta")]
        public double? Delta { get; set; }

        [Column("groupe")]
        public int? Groupe { get; set; }

        [Column("info")]
        public int? Info { get; set; }

        [Column("libelle")]
        [StringLength(50)]
        public string? Libelle { get; set; }

        [Column("seuil_bas")]
        public double? SeuilBas { get; set; }

        [Column("seuil_haut")]
        public double? SeuilHaut { get; set; }

        [Column("priorite")]
        [StringLength(20)]
        public string? Priorite { get; set; }

        [Column("station_id")]
        public int StationId { get; set; }

        [Column("unite")]
        [StringLength(20)]
        public string? Unite { get; set; }

        [Column("numero")]
        public int? Numero { get; set; }

        [Column("ordre")]
        public int? Ordre { get; set; }

        [Column("voie_enregistree")]
        public int? VoieEnregistree { get; set; }

        [Column("voie_stockee")]
        public int? VoieStockee { get; set; }

        [Column("actif")]
        public bool? Actif { get; set; }

        [Column("virgule")]
        public int? Virgule { get; set; }

        [Column("parametre1")]
        public int? Parametre1 { get; set; }

        [Column("parametre2")]
        public int? Parametre2 { get; set; }

        [Column("parametre3")]
        public int? Parametre3 { get; set; }

        [Column("parametre4")]
        public int? Parametre4 { get; set; }

        [Column("parametre5")]
        public int? Parametre5 { get; set; }

        [Column("parametre6")]
        public int? Parametre6 { get; set; }

        [Column("parametre7")]
        public int? Parametre7 { get; set; }

        [Column("parametre8")]
        public int? Parametre8 { get; set; }

        [Column("parametre9")]
        public int? Parametre9 { get; set; }

        [Column("parametre10")]
        public int? Parametre10 { get; set; }

        [Column("traitement_id")]
        public int TraitementId { get; set; }

        [Column("abonnements")]
        public long? Abonnements { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Models.Entities
{
    public class Station
    {
        #region DB
        public int Id { get; set; }
        #endregion
        public string Initiales { get; set; }
        #region FTP
        public string? FtpAdresse { get; set; }
        public string? FtpUserName { get; set; }
        public string? FtpPassword { get; set; }
        #endregion

        #region ObjStation
        #endregion

        #region objStation
        /// <summary>
        /// Numero station
        /// </summary>
        public int n_sta { get; set; }
        public int n_enrg { get; set; }
        public int CadScrut { get; set; }
        public int CadEnrg { get; set; }
        public int n_vint_u { get; set; }
        public int n_et_u { get; set; }
        public int nu_enrg { get; set; }

        public string? BassinVersant { get; set; }
        public string? METEO { get; set; }
        public string? Evt_encours { get; set; } // pluie (O) ou non (N) ou invalide (I) au moment de l'appel
        /// <summary>
        /// Oui(O) ou Non(N) ou invalide() au moment de l'appel
        /// </summary>
        public string? Critique { get; set; }

        private List<VoiesAnalogiques>? _vint = new List<VoiesAnalogiques>();
        /// <summary>
        /// Retourne une liste d'éléments (61 éléments par défaut). "Public VInt(60) As structVoies1".
        /// Assigne une liste d'éléments. 
        /// </summary>
        public List<VoiesAnalogiques>? VInt
        {
            get
            {
                if (_vint == null)
                {
                    _vint = new List<VoiesAnalogiques>();
                    for (var i = 0; i < 61; i++) // Public VInt(60) As structVoies1
                    {
                        _vint.Add(new VoiesAnalogiques());
                    }
                }
                return _vint;
            }
            set { _vint = value; }
        }

        private List<VoiesAnalogiques>? _venrg = null;
        /// <summary>
        /// Retourne une liste d'éléments (33 éléments par défaut). "Public VEnrg(33) As structVoies1"
        /// Assigne une liste d'éléments.
        /// </summary>
        public List<VoiesAnalogiques>? VEnrg
        {
            get
            {
                if (_venrg == null)
                {
                    _venrg = new List<VoiesAnalogiques>();
                    for (var i = 0; i < 34; i++) // Public VEnrg(33) As structVoies1
                    {
                        _venrg.Add(new VoiesAnalogiques());
                    }
                }
                return _venrg;
            }
            set { _venrg = value; }
        }
        public int[]? OrdreVint { get; set; } // Size: 34

        private List<VoiesEtat>? _vetat = null;
        /// <summary>
        /// Retourne une liste d'éléments (18 éléments par défaut). "Public VEtat(17) As structVoies2"
        /// Assigne une liste d'éléments;
        /// </summary>
        public List<VoiesEtat>? VEtat
        {
            get
            {
                if (_vetat == null)
                {
                    _vetat = new List<VoiesEtat>();
                    for (var i = 0; i < 18; i++)
                    {
                        _vetat.Add(new VoiesEtat());
                    }
                }
                return _vetat;
            }
            set { _vetat = value; }
        }

        private List<VoiesTor>? _vtor = null;

        /// <summary>
        /// Retourne la liste d'éléments de voies Tor
        /// </summary>
        public List<VoiesTor>? VTor
        {
            get
            {
                if (_vtor == null)
                {
                    _vtor = new List<VoiesTor>();
                }
                return _vtor;
            }
            set { _vtor = value; }
        }

        public int VersionDialog { get; set; }
        public string? VersionEnregistrement { get; set; }
        public string? VersionEnrg { get; set; }
        public string? Taches { get; set; }

        private List<string>? _tableJours = null;
        /// <summary>
        /// Retourne la liste des tables du jour (65 éléments par défaut) "Public TableJours(64) As String"
        /// </summary>
        public List<string> TableJours
        {
            get
            {
                if (_tableJours == null)
                {
                    _tableJours = Enumerable.Repeat("", 65).ToList();
                }
                return _tableJours;
            }
            set { _tableJours = value; }
        }

        private List<int>? _tableEvts = null;
        public List<int> TableEvts
        {
            get
            {
                if (_tableEvts == null)
                {
                    _tableEvts = Enumerable.Repeat(0, 65).ToList();
                }
                return _tableEvts;
            }
            set { _tableEvts = value; }
        }

        private List<int>? _tableMots = null;
        public List<int> TableMots
        {
            get
            {
                if (_tableMots == null)
                {
                    _tableMots = Enumerable.Repeat(0, 65).ToList();
                }
                return _tableMots;
            }
            set { _tableMots = value; }
        }

        public DateTime DateDerTrfSta { get; set; }
        public DateTime DateDerTrfBase { get; set; }
        public DateTime DateMajSta { get; set; }
        public DateTime DateMajBase { get; set; }

        public int Perte_type { get; set; }
        public int Perte_voie { get; set; }
        public string? Perte_nom { get; set; }
        public bool Perte_val { get; set; }

        public double EcartHorloges { get; set; }

        public DateTime DateGo { get; set; }
        public DateTime DateInit { get; set; }
        public DateTime DateAcq { get; set; }
        public DateTime DateEnrg { get; set; }
        public DateTime DateStop { get; set; }
        public DateTime? Perte_Deb { get; set; }
        public DateTime? Perte_Fin { get; set; }
        public string? Perte_Defaut { get; set; }
        public string? Perte_Cause { get; set; }
        public string? Perte_Remede { get; set; }

        public bool ArretEnrg { get; set; }
        public bool DefautDateParam { get; set; }

        public int PcMemoire { get; set; }

        public int CrCapteur { get; set; }
        public int CrEtat { get; set; }
        public int DefautCapteur { get; set; }
        public bool DefautEtat { get; set; }
        #endregion

        #region Constructor
        public Station(string initiales)
        {
            Initiales = initiales;
            VInt = null;
            for (var i = 0; i < 61; i++)
            {
                VInt![i].Num = i;
            }
            VEnrg = null;
            for (var i = 0; i < 33; i++)
            {
                VEnrg![i].Num = i;
            }
            VEtat = null;
            for (var i = 0; i < 17; i++)
            {
                VEtat![i].ETlibel = $"Voie etat {i}";
            }
        }

        public Station()
        : this("Defaut")
        { }
        #endregion
    }
}

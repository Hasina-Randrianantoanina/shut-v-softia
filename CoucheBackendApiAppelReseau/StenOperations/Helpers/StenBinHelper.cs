

using StenOperations.Models.Entities;

namespace StenOperations.Helpers
{
    /// <summary>
    /// Class used to convert STEN bin file to object of type ContextMesure
    /// </summary>
    public class StenBinHelper
    {
        public int ErrorCode { get; private set; }
        public string ErrorText { get; private set; } = "";

        private Dictionary<int, string> _evts = new Dictionary<int, string>()
        {
            { 0 , " " }, // Mesure
            { 1 , "J" }, // Evt journalier
            { 2 , "I" }, // Evt initial
            { 3 , "H" }, // Modif heure
            { 4 , "U" }, // Modif seuil
            { 5 , "V" }, // Modif delta
            { 6 , "C" }, // Modif cadence
            { 7 , "N" }, // Efface mémoire
            { 8 , "L" }, // Vidage
            { 9 , "K" },   // V21.1 - Modif 100%
            { 10, "Q" }, // Modif config
            { 11, "X" }, // Reset
            { 12, "S" }, // Changement TS
            { 13, "K" }, // Modif 0%
            { 14, "&" }, // Changement TC
            { 15, "?" }, // Evt inconnu
            { 16, "?" }, // Evt inconnu
            { 17, "B" }, // Boucl jour
            { 18, "M" }, // Boucl mémoire
            { 19, "Q" },
            { 20, "Y" }, // Coupure secteur
            { 21, "$" }, // Commentaire
            { 22, "G" },   // V21.1 - Mesure
            { 23, "P" },   // V21.1 - Mesure
            { 24, "T" },   // V21.1 - Mesure
            { 25, "R" }, // Enrg TS chgt jour
            { 32, "E" }, // Chgt jour
            { 33, "O" }, // Enrg TC chgt jour
            { 37, "$" },   // V21.2 - Commentaire
            //{ XX, "!"}
        };

        private string Evt(int evt_type)
        {
            if (_evts.ContainsKey(evt_type))
                return _evts[evt_type];
            return "!";
        }

        private string Hex(int number)
        {
            return number.ToString("X");
        }

        /// <summary>
        /// Return the list of measures
        /// </summary>
        /// <param name="nomFicBin">Path of the bin file</param>
        /// <param name="station"></param>
        /// <param name="dateDebut"></param>
        /// <returns></returns>
        public ContextMesure GetMesures(string nomFicBin, Station station, DateTime dateDebut)
        {
            return DecodeBin(nomFicBin, station, jourEnCours: DateTime.Now, jourCourantX: dateDebut);
        }

        private ContextMesure DecodeBin(string nomFicBin, Station station, DateTime jourEnCours, DateTime jourCourantX)
        {
            var context = new ContextMesure() { Station = station };
            // Check if file exists    
            if (!File.Exists(nomFicBin))
            {
                ErrorCode = 2005;
                ErrorText = $"Fichier {nomFicBin} introuvable";
                return context;
            }
            // Read all bytes of data
            var dataSrc = File.ReadAllBytes(nomFicBin);
            if (dataSrc == null || dataSrc.Length == 0)
            {
                ErrorCode = 2005;
                ErrorText = $"Fichier {nomFicBin} vide";
                return context;
            }



            var code_voie = new string[33];
            for (var i = 0; i <= 26; i++)
            {
                code_voie[i] = ((char)(i + 96)).ToString();
            }
            code_voie[27] = "(";
            code_voie[28] = ")";
            code_voie[29] = "{";
            code_voie[30] = "}";
            code_voie[31] = "[";
            code_voie[32] = "]";

            // Init variables
            var posTmpCourante = 0;
            var posEvtsLigE = 0;
            var posLigD = 0;
            var tableauTmp = new string[65537];

            // Ecriture des lignes #
            var ligne = "";
            var virgules = new int[station.n_enrg + 1];
            for (var i = 1; i <= station.n_enrg; i++)
            {
                var venrg = station.VEnrg![i];
                if (venrg.Num == 0)
                {
                    return context;
                }
                ligne = (venrg.Virgule > 4 || venrg.Virgule < 0) ? "0" : (-4 + venrg.Virgule).ToString();
                int.TryParse(ligne, out var virg);
                virgules[i] = virg;
                ligne = $"#{code_voie[i]}\t{venrg.Libel}\t{ligne}";

                var header = new HeaderMesure()
                {
                    Code = code_voie[i],
                    Libelle = venrg.Libel,
                    Virgule = virg,
                };
                context.Headers.Add(header);

                posTmpCourante++;
                tableauTmp[posTmpCourante] = ligne;
            }

            // Ecriture date
            var nops = station.n_sta;
            jourCourantX = DateTime.FromOADate((int)jourCourantX.ToOADate());
            ligne = $"D{jourCourantX.ToString("dd/MM/yyyy 00\\h00")} {nops.ToString("D3")}";
            context.CurrentTime = jourCourantX;
            posTmpCourante++;
            tableauTmp[posTmpCourante] = ligne;
            posLigD = posTmpCourante;

            //****** Enregistrement TC fonction 14 à développer
            int va, vb, vc;
            var nb_evts = 0;
            var pres_voie = new int[34];

            var nb_voies = 0;
            var evt_type = 0;
            var heure = "";
            var index = 0;
            do
            {
                nb_evts++;

                va = dataSrc[index]; index++; // minute
                if (va == 255) { ErrorCode = 2007; }
                va = (va & 0xF) + ((int)(va / 16)) * 10;
                ligne = $"{va.ToString("D2")} ";
                int currentMin = va;

                va = dataSrc[index]; index++; // heure
                if (va == 255) { ErrorCode = 2007; }
                va = (va & 0xF) + ((int)(va / 16)) * 10;
                heure = $"{va.ToString("D2")}h{ligne}";
                
                var currentHour = va;

                nb_voies = dataSrc[index]; index++; // nb_voies
                evt_type = dataSrc[index]; index++; // fonction

                va = dataSrc[index]; index++; // 00 XX 00 00
                for (var i = 0; i < 8; i++)
                {
                    pres_voie[i + 17] = Convert.ToInt32((va & ((int)Math.Pow(2, i))) != 0);
                }
                va = dataSrc[index]; index++; // XX 00 00 00
                for (var i = 0; i < 8; i++)
                {
                    pres_voie[i + 25] = Convert.ToInt32((va & ((int)Math.Pow(2, i))) != 0);
                }
                va = dataSrc[index]; index++; // 00 00 00 XX
                for (var i = 0; i < 8; i++)
                {
                    pres_voie[i + 1] = Convert.ToInt32((va & ((int)Math.Pow(2, i))) != 0);
                }
                va = dataSrc[index]; index++; // 00 00 XX 00
                for (var i = 0; i < 8; i++)
                {
                    pres_voie[i + 9] = Convert.ToInt32((va & ((int)Math.Pow(2, i))) != 0);
                }

                if (ErrorCode == 2007) { evt_type = 255; }

                switch (evt_type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 8:
                    case 22:
                    case 23:
                    case 24: // _, J, I, L, G, P, T
                        {
                            // J: Evt journalier
                            // I: Evt initial
                            // L: Vidage
                            // G: Mesure
                            // P: Mesure
                            // T: Mesure
                            ligne = $"{Evt(evt_type)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;

                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));

                                    if (station.VEnrg![i].Num != 0)
                                    {
                                        ligne = $"{ligne}{code_voie[i]}{va.ToString("D4")} ";

                                        var currentMesure = new Mesure()
                                        {
                                            Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day, 
                                                currentHour, currentMin, 0),
                                            CodeVoie = code_voie[i],
                                            NomVoie = station.VEnrg[i].Libel,
                                            NumeroVoie = station.VEnrg[i].Num,
                                            //Valeur = va,
                                            ValeurBrute = va.ToString("D4"),
                                            Virgule = virgules[i],
                                            TypeEvt = Evt(evt_type),
                                        };
                                        ev.Mesures.Add(currentMesure);
                                    }
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 4:
                    case 5: // U, V
                        {
                            // U: Modif seuil
                            // V: Modif delta
                            ligne = $"{Evt(evt_type)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;

                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                                    if (station.VEnrg![i].Num != 0)
                                    {
                                        ligne = $"{ligne}{code_voie[i]}{va.ToString("D4")} ";

                                        var currentMesure = new Mesure()
                                        {
                                            Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                            CodeVoie = code_voie[i],
                                            NomVoie = station.VEnrg[i].Libel,
                                            NumeroVoie = station.VEnrg[i].Num,
                                            //Valeur = va,
                                            ValeurBrute = va.ToString("D4"),
                                            Virgule = virgules[i],
                                            TypeEvt = Evt(evt_type),
                                        };
                                        ev.Mesures.Add(currentMesure);
                                    }
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 6: // C: Modif cadences
                        {
                            ligne = $"{Evt(evt_type)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;

                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                                    var vaStr = (va + 10000).ToString();
                                    ligne = $"{ligne}{code_voie[i]}{vaStr.Substring(vaStr.Length - 4)} ";

                                    // Add Cadences
                                    var currentMesure = new Mesure()
                                    {
                                        Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                            currentHour, currentMin, 0),
                                        CodeVoie = code_voie[i],
                                        //NomVoie = station.VEnrg[i].Libel,
                                        //NumeroVoie = station.VEnrg[i].Num,
                                        ValeurBrute = vaStr.Substring(vaStr.Length - 4),
                                        TypeEvt = Evt(evt_type),
                                    };
                                    ev.Mesures.Add(currentMesure);
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 12:
                    case 14:
                    case 25:
                    case 33: // S, &, R, O
                        {
                            // S: Changement TS (ETor)
                            // &:
                            // R: Enrg TS chgt jour
                            // O: Enrg TC chgt jour
                            ligne = $"{Evt(evt_type)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;
                                    ligne = $"{ligne}{code_voie[i]}";
                                    var vahex = $"0{Hex(va)}";
                                    ligne = $"{ligne}{vahex.Substring(vahex.Length - 2)}";
                                    var vbhex = $"0{Hex(vb)}";
                                    ligne = $"{ligne}{vbhex.Substring(vbhex.Length - 2)} ";

                                    // Add Changement TOR
                                    var currentMesure = new Mesure()
                                    {
                                        Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                            currentHour, currentMin, 0),
                                        CodeVoie = code_voie[i],
                                        //NomVoie = station.VEnrg[i].Libel,
                                        //NumeroVoie = station.VEnrg[i].Num,
                                        ValeurBrute = $"{vahex.Substring(vahex.Length - 2)}{vbhex.Substring(vbhex.Length - 2)}",
                                        TypeEvt = Evt(evt_type),
                                    };
                                    ev.Mesures.Add(currentMesure);
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 3: // H: Modif heure
                        {
                            ligne = $"{Evt(evt_type)}{heure}";

                            vb = dataSrc[index]; index++; // minutes
                            va = dataSrc[index]; index++; // heure
                            var vaval = ((va & 0xF) + ((int)(va / 16) * 10));
                            var vbval = ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{ligne}{vaval.ToString("00/")}";
                            ligne = $"{ligne}{vbval.ToString("00/")}";
                            var min = vbval.ToString("00");
                            var hh = vaval.ToString("00");

                            vb = dataSrc[index]; index++; // jour
                            va = dataSrc[index]; index++; // secondes
                            vaval = ((va & 0xF) + ((int)(va / 16) * 10));
                            vbval = ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{ligne}{vaval.ToString("00 ")}";
                            ligne = $"{ligne}{vbval.ToString("00/")}";
                            var sec = vaval.ToString("00");
                            var jj = vbval.ToString("00");

                            vb = dataSrc[index]; index++; // année
                            va = dataSrc[index]; index++; // mois
                            vaval = ((va & 0xF) + ((int)(va / 16) * 10));
                            vbval = ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{ligne}{vaval.ToString("00/")}";
                            ligne = $"{ligne}{vbval.ToString("00")}";
                            var aa = vbval.ToString("00");
                            var mm = vaval.ToString("00");

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            var currentMesure = new DateMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                            currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                                ValeurBrute = $"{jj}/{mm}/{aa} {hh}:{min}:{sec}"
                            };
                            ev.Mesures.Add(currentMesure);

                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 7:
                    case 17:
                    case 18:
                    case 21:
                    case 37: // N, B, M, $, (0x25)
                        {
                            // N: efface la mémoire
                            // B: ??? Boucl jour
                            // M: ??? Boucl mémoire
                            // $: défaut de ligne
                            ligne = $"{Evt(evt_type)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                            currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);
 
                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;
                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                                    var va10000 = (va + 10000).ToString();
                                    ligne = $"{ligne}{code_voie[i]}{va10000.Substring(va10000.Length - 4)} ";

                                    var currentMesure = new Mesure()
                                    {
                                        Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                        TypeEvt = Evt(evt_type),
                                        CodeVoie = code_voie[i],
                                        //NomVoie = station.VEnrg[i].Libel,
                                        //NumeroVoie = station.VEnrg[i].Num,
                                        ValeurBrute = va10000.Substring(va10000.Length - 4),
                                    };
                                    ev.Mesures.Add(currentMesure);
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 10: // Q
                        {
                            // evt_type = 19 // Q Modif config
                            ligne = $"{Evt(evt_type)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;
                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                                    var va10000 = (va + 10000).ToString();
                                    ligne = $"{ligne}{code_voie[i]}{va10000.Substring(va10000.Length - 4)} ";

                                    var currentMesure = new Mesure()
                                    {
                                        Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                        currentHour, currentMin, 0),
                                        TypeEvt = Evt(evt_type),
                                        CodeVoie = code_voie[i],
                                        //NomVoie = station.VEnrg[i].Libel,
                                        //NumeroVoie = station.VEnrg[i].Num,
                                        ValeurBrute = va10000.Substring(va10000.Length - 4),
                                    };
                                    ev.Mesures.Add(currentMesure);
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 11:
                    case 20: // X, Y
                        {
                            ligne = $"{Evt(evt_type)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                        currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 13: // K 13; K 9 - Modif 0% et 100%
                        {
                            ligne = $"{Evt(9)}{heure}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                        currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            for (var i = 0; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;
                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                                    vc = va; // Modif 0%

                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;
                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10)); // Modif 100%

                                    ligne = $"{ligne}{code_voie[i]}{vc.ToString("0000 ")}";
                                    ligne = $"{ligne}{va.ToString("0000 ")}";

                                    var currentMesure = new ZeroCentMesure()
                                    {
                                        Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                currentHour, currentMin, 0),
                                        TypeEvt = Evt(evt_type),
                                        CodeVoie = code_voie[i],
                                        //NomVoie = station.VEnrg[i].Libel,
                                        //NumeroVoie = station.VEnrg[i].Num,
                                        Virgule = virgules[i], //station.VEnrg[i].Virgule, 
                                        ZeroBrute = vc.ToString("0000"),
                                        CentBrute = va.ToString("0000"),
                                    };
                                    ev.Mesures.Add(currentMesure);
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 32: // E
                        {
                            nb_evts--;
                            heure = "00h00 ";

                            vb = dataSrc[index]; index++; // mois
                            va = dataSrc[index]; index++; // jour
                            var vaval = ((va & 0xF) + ((int)(va / 16) * 10));
                            var vbval = ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{vaval.ToString("00/")}";
                            ligne = $"{ligne}{vbval.ToString("00/")}";

                            vb = dataSrc[index]; index++; // année
                            va = dataSrc[index]; index++; // année

                            va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                            vb = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{ligne}{vb.ToString("0000")}  ";
                            DateTime.TryParse(ligne, out var dateLigne);
                            jourCourantX = dateLigne;

                            ligne = $"{Evt(evt_type)}{ligne}";
                            vb = dataSrc[index]; index++;
                            va = dataSrc[index]; index++;

                            va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                            va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));

                            var va10000 = (va + 10000).ToString();
                            ligne = $"{ligne}{va10000.Substring(va10000.Length - 4)}";

                            var ev = new EventMesure()
                            {
                                Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                currentHour, currentMin, 0),
                                TypeEvt = Evt(evt_type),
                            };
                            context.Events.Add(ev);

                            var currentMesure = new Mesure()
                            { 
                                Time = jourCourantX,
                                TypeEvt = Evt(evt_type),
                                ValeurBrute = va10000.Substring(va10000.Length - 4),
                            };
                            ev.Mesures.Add(currentMesure);

                            // comptage à 0
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;

                            if (posEvtsLigE == 0)
                            {   // 1re ligne E
                                ligne = $"D{jourCourantX.ToString("dd/MM/yyyy  00\\h00  ")}{nops.ToString("000")}";
                                tableauTmp[posLigD] = ligne;
                                context.CurrentTime = jourCourantX;
                            }
                            else
                            {
                                var liii = tableauTmp[posEvtsLigE];
                                tableauTmp[posEvtsLigE] = liii.Replace("0000", nb_evts.ToString("0000"));
                            }
                            posEvtsLigE = posTmpCourante;
                            nb_evts = 1;
                        }
                        break;
                    default: // inclue le code 255 en erreur
                        {
                            var cpt = 0;
                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0) { cpt++; }
                            }
                            if (ErrorCode == 0 && cpt == nb_voies) // fonction inconnue (!) mais cohérence
                            {
                                // affichage mais pas d'enregistrement des valeurs qui suivent
                                ligne = $"{Evt(evt_type)}{heure}";

                                var ev = new EventMesure()
                                {
                                    Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                    currentHour, currentMin, 0),
                                    TypeEvt = Evt(evt_type),
                                };
                                context.Events.Add(ev);

                                for (var i = 1; i <= 32; i++)
                                {
                                    if (pres_voie[i] != 0)
                                    {
                                        vb = dataSrc[index]; index++;
                                        va = dataSrc[index]; index++;

                                        va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                        va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));

                                        var va10000 = (va + 10000).ToString();
                                        ligne = $"{ligne}{va10000.Substring(va10000.Length - 4)} ";

                                        var currentMesure = new Mesure()
                                        {
                                            Time = new DateTime(context.CurrentTime.Year, context.CurrentTime.Month, context.CurrentTime.Day,
                                                    currentHour, currentMin, 0),
                                            TypeEvt = Evt(evt_type),
                                            ValeurBrute = va10000.Substring(va10000.Length - 4),
                                        };
                                        ev.Mesures.Add(currentMesure);
                                    }
                                }
                                posTmpCourante++;
                                tableauTmp[posTmpCourante] = ligne;
                            }
                            else
                            {
                                ErrorText = "Défaut dans result.bin";
                                goto exit_loop;
                            }
                        }
                        break;
                } // switch (evt_type)

            }
            while (index < dataSrc.Length - 8);

        exit_loop:;

            var li = tableauTmp[posEvtsLigE];
            tableauTmp[posEvtsLigE] = li.Replace("0000", nb_evts.ToString("0000"));
            if (ErrorCode == 0)
            {
                if (jourEnCours > jourCourantX)
                {
                    ligne = $"F{jourCourantX.ToString("dd/MM/yyyy")}  23h59";
                }
                else
                {
                    var strH = $"{jourCourantX.ToString("dd/MM/yyyy")} {heure.Substring(0, 2)}:{heure.Substring(3, 2)}:00";
                    DateTime.TryParse(strH, out var dateH);
                    jourCourantX = dateH.AddMinutes(1);
                    ligne = $"F{jourCourantX.ToString("dd/MM/yyyy HH\\hmm")}";
                }
                posTmpCourante++;
                tableauTmp[posTmpCourante] = ligne;
            }

            return context;
        }

    }
}

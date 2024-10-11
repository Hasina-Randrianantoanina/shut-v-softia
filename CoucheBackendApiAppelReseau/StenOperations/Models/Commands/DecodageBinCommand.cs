using System.Diagnostics;
using System.Text;
using StenOperations.Logger;
using StenOperations.Models.Entities;


namespace StenOperations.Models.Commands
{
    public class DecodageBinCommand : DefaultCommand
    {
        private Dictionary<int, string> _evts = new Dictionary<int, string>()
        {
            { 0 , " " },
            { 1 , "J" },
            { 2 , "I" },
            { 3 , "H" },
            { 4 , "U" },
            { 5 , "V" },
            { 6 , "C" },
            { 7 , "N" },
            { 8 , "L" },
            { 9 , "K" },   // V21.1
            { 10, "Q" },
            { 11, "X" },
            { 12, "S" },
            { 13, "K" },
            { 14, "&" },
            { 15, "?" },
            { 16, "?" },
            { 17, "B" },
            { 18, "M" },
            { 19, "Q" },
            { 20, "Y" },
            { 21, "$" },
            { 22, "G" },   // V21.1
            { 23, "P" },   // V21.1
            { 24, "T" },   // V21.1
            { 25, "R" },
            { 32, "E" },
            { 33, "O" },
            { 37, "$" },   // V21.2
            //{ XX, "!"}
        };

        private string Evt(int evt_type)
        {
            if (_evts.ContainsKey(evt_type))
                return _evts[evt_type];
            return "!";
        }

        private static string Hex(int number)
        {
            return number.ToString("X");
        }

        public DecodageBinCommand(ILogger logger, Station station)
            :base(logger, station) 
        {
        }

        public override void Execute()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var jourEnCours = DateTime.FromOADate((int) DateTime.Now.ToOADate());
            DecodeBin(Context.NomFicBin, Station.n_sta, jourEnCours, Context.DateDebut);
            stopWatch.Stop();
            Logger.Log("decode bin->{0} {1}s", Context.NomFicMan, (double) (stopWatch.ElapsedMilliseconds / 1000.0));
        }

        private void DecodeBin(string nomFicBin, int nops, DateTime jourEnCours, DateTime jourCourantX)
        {
            // Read all bytes of data
            var dataSrc = File.ReadAllBytes(nomFicBin);
            if (dataSrc == null || dataSrc.Length == 0)
            {
                ErrorCode = 2005;
                ErrorText = $"Fichier {nomFicBin} vide";
                return;
            }

            // Init NomFicMan,
            var nomFicMan = "";
            if (jourCourantX.Month < 10)
            {
                nomFicMan = jourCourantX.ToString("ddMyy");
            }
            else if (jourCourantX.Month == 10)
            {
                nomFicMan = jourCourantX.ToString("dd\\Oyy");
            }
            else if (jourCourantX.Month == 11)
            {
                nomFicMan = jourCourantX.ToString("dd\\Nyy");
            }
            else if (jourCourantX.Month == 12)
            {
                nomFicMan = jourCourantX.ToString("dd\\Dyy");
            }
            // TODO: on initialise où nu_enrg: dans la base de données
            nomFicMan = $"{Context.RootDirPath}/{Station.Initiales.Substring(0, 2)}{Station.nu_enrg.ToString("D1")}{nomFicMan}.MAN";
            if (File.Exists(nomFicMan)) 
            {
                File.Delete(nomFicMan);
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
            for (var i = 1; i <= Station.n_enrg; i++)
            {
                var venrg = Station.VEnrg![i];
                if (venrg.Num == 0)
                {
                    return; 
                }
                ligne = (venrg.Virgule > 4 || venrg.Virgule < 0) ? "0" : (-4 + venrg.Virgule).ToString();
                ligne = $"#{code_voie[i]}\t{venrg.Libel}\t{ligne}";
                
                posTmpCourante++;
                tableauTmp[posTmpCourante] = ligne;
            }

            // Ecriture date
            jourCourantX = DateTime.FromOADate((int)jourCourantX.ToOADate());
            ligne = $"D{jourCourantX.ToString("dd/MM/yyyy 00\\h00")} {nops.ToString("D3")}";
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

                va = dataSrc[index]; index++; // heure
                if (va == 255) { ErrorCode = 2007; }
                va = (va & 0xF) + ((int)(va / 16)) * 10;
                heure = $"{va.ToString("D2")}h{ligne}";

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
                    case 0: case 1: case 2: case 8: case 22: case 23: case 24: // _, J, I, L, G, P, T
                        {
                            ligne = $"{Evt(evt_type)}{heure}";
                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;

                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));

                                    if (Station.VEnrg![i].Num != 0)
                                    {
                                        ligne = $"{ligne}{code_voie[i]}{va.ToString("D4")} ";
                                    }
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 4: case 5: // U, V
                        {
                            ligne = $"{Evt(evt_type)}{heure}";
                            for (var i = 1; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;

                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                                    if (Station.VEnrg![i].Num != 0)
                                    {
                                        ligne = $"{ligne}{code_voie[i]}{va.ToString("D4")} ";
                                    }
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 6: // C cadences
                        {
                            ligne = $"{Evt(evt_type)}{heure}";
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
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 12: case 14: case 25: case 33: // S, &, R, O
                        {
                            ligne = $"{Evt(evt_type)}{heure}";
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
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 3: // H
                        {
                            ligne = $"{Evt(evt_type)}{heure}";

                            vb = dataSrc[index]; index++; // minutes
                            va = dataSrc[index]; index++; // heure
                            var vaval = ((va & 0xF) + ((int)(va / 16) * 10));
                            var vbval = ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{ligne}{vaval.ToString("00/")}";
                            ligne = $"{ligne}{vbval.ToString("00/")}";

                            vb = dataSrc[index]; index++; // jour
                            va = dataSrc[index]; index++; // secondes
                            vaval = ((va & 0xF) + ((int)(va / 16) * 10));
                            vbval = ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{ligne}{vaval.ToString("00 ")}";
                            ligne = $"{ligne}{vbval.ToString("00/")}";

                            vb = dataSrc[index]; index++; // année
                            va = dataSrc[index]; index++; // mois
                            vaval = ((va & 0xF) + ((int)(va / 16) * 10));
                            vbval = ((vb & 0xF) + ((int)(vb / 16) * 10));
                            ligne = $"{ligne}{vaval.ToString("00/")}";
                            ligne = $"{ligne}{vbval.ToString("00")}";

                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 7: case 17: case 18: case 21: case 37: // N, B, M, $, (0x25)
                        {
                            ligne = $"{Evt(evt_type)}{heure}";
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
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 10: // Q
                        {
                            // evt_type = 19 // Q
                            ligne = $"{Evt(evt_type)}{heure}";
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
                                }
                            }
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 11: case 20: // X, Y
                        {
                            ligne = $"{Evt(evt_type)}{heure}";
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;
                        }
                        break;
                    case 13: // K 13; K 9
                        {
                            ligne = $"{Evt(9)}{heure}";
                            for (var i = 0; i <= 32; i++)
                            {
                                if (pres_voie[i] != 0 && index < dataSrc.Length)
                                {
                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;
                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));
                                    vc = va;

                                    vb = dataSrc[index]; index++;
                                    va = dataSrc[index]; index++;
                                    va = ((va & 0xF) + ((int)(va / 16) * 10)) * 100;
                                    va = va + ((vb & 0xF) + ((int)(vb / 16) * 10));

                                    ligne = $"{ligne}{code_voie[i]}{vc.ToString("0000 ")}";
                                    ligne = $"{ligne}{va.ToString("0000 ")}";
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
                            // comptage à 0
                            posTmpCourante++;
                            tableauTmp[posTmpCourante] = ligne;

                            if (posEvtsLigE == 0)
                            {   // 1re ligne E
                                ligne = $"D{jourCourantX.ToString("dd/MM/yyyy  00\\h00  ")}{nops.ToString("000")}";
                                tableauTmp[posLigD] = ligne;
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
            if (posTmpCourante >= 0)
            {
                var sb = new StringBuilder();
                for (var i = 1; i <= posTmpCourante; i++) 
                {
                    sb.AppendLine(tableauTmp[i]);
                }
                File.WriteAllText(nomFicMan, sb.ToString());
                Context.NomFicMan = nomFicMan;
            }
        }

    }
}

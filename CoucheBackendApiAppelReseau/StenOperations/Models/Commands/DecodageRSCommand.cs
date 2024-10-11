
using StenOperations.Logger;
using StenOperations.Models.Entities;

namespace StenOperations.Models.Commands
{
    public class DecodageRSCommand: DefaultCommand
    {
        public string NomFicRep { get; set; }

        //public int JourCourant { get; set; }
        public int JourPrecedent { get; set; }
        public int JourBouclage { get; set; }
        public int NbEvtsJourCourant { get; set; }
        public int NbEvtsJourPrecedent { get; set; }
        public int NbMotsJourCourant { get; set; }
        public int NbMotsJourPrecedent { get; set; }
        //public bool FicBinRecu { get; set; } = false;

        public DecodageRSCommand(ILogger logger, string nomFicRep, Station station)
            : base(logger, station) 
        {
            NomFicRep = nomFicRep;
        }

        public override void Execute()
        {
            Logger.Log($"-> Decoding: {NomFicRep}");
            var line = "";
            using (var sr = new StreamReader(NomFicRep)) 
            {
                while (!sr.EndOfStream) 
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }    
                    var cr = DecodageRS(line!);
                    Logger.Log(cr);
                    if (cr.Substring(0, 3) == "Err" && Context.Operation != "FIN")
                    {
                        ErrorCode = 10;
                        ErrorText = cr;
                        Context.Operation = "FIN";
                        Context.FicBinRecu = false;
                    }
                }
            }
            // TODO: remove comment on real test/prod
            //File.Delete(NomFicRep);
        }

        #region Init TextCde
        private string TexteCde(string cde)
        {
            var texteCde = "";
            if (cde == "Q")
            {
                texteCde = "fermeture session";
            }
            else if (cde.Substring(0, 3) == "AAA")
            {
                texteCde = "ouverture session";
            }
            else if (cde.Substring(0, 4) == "9000")
            {
                texteCde = "connexion mcx";
            }
            else if (cde.Substring(0, 2) == "AT")
            {
                texteCde = "modem RC cde hayes";
            }
            else
            {
                var head = cde.Substring(1, 3);
                switch (head)
                {
                    case "L39":
                        texteCde = "date téléchargement";
                        break;
                    case "L36":
                        texteCde = "état des tâches";
                        break;
                    case "L34":
                        texteCde = "date-heure station";
                        break;
                    case "L26":
                        texteCde = "date de dernier vidage(transfert)";
                        break;
                    case "L04":
                        texteCde = "ordre des voies internes";
                        break;
                    case "L10":
                        texteCde = "cadences et ordre des voies enregistrées";
                        break;
                    case "L23":
                        texteCde = "voies internes";
                        break;
                    case "L14":
                        texteCde = "ordre des voies etats";
                        break;
                    case "L29":
                        texteCde = "voies etat";
                        break;
                    case "L60":
                        texteCde = "date GO";
                        break;
                    case "L61":
                        texteCde = "date Init";
                        break;
                    case "L62":
                        texteCde = "date acq";
                        break;
                    case "L63":
                        texteCde = "date Enrg";
                        break;
                    case "L64":
                        texteCde = "date stop";
                        break;
                    case "L25":
                        texteCde = "infos tps réel";
                        break;
                    case "L35":
                        texteCde = "date du paramètrage";
                        break;
                    case "L30":
                        texteCde = "table des jours - date";
                        break;
                    case "L31":
                        texteCde = "table des jours - nb evts";
                        break;
                    case "L32":
                        texteCde = "table des jours - nb mots";
                        break;
                    case "L33":
                        texteCde = "evts/mots dans 1 jour";
                        break;
                    case "E40":
                        texteCde = "fenêtre transfert de rattrapage";
                        break;
                    case "M41":
                        texteCde = "transfert de rattrapage T & #";
                        break;
                    case "M42":
                        texteCde = "transfert normal T & #";
                        break;
                    case "M44":
                        texteCde = "transfert de rattrapage T ";
                        break;
                    case "E43":
                        texteCde = "fenêtre transfert normal";
                        break;
                    default:
                        texteCde = "texte cde ???";
                        break;
                }
            }
            return texteCde;
        }

        #endregion

        #region DecodageRS
        private string DecodageRS(string message)
        {
            var typeReponse = message.Substring(1, 3);
            var mess = message.Substring(4);
            var texteMessage = TexteCde(message.Substring(0, 4));

            if (message.Substring(0, 2) == ">D")
            {
                Station.VersionEnrg = message.Substring(1, 11); // v2.6
                var vd = 0;
                int.TryParse(message.Substring(2, 2), out vd);
                Station.VersionDialog = vd;
                Station.VersionEnregistrement = message.Substring(4, 3);
                return message;
            }

            switch(typeReponse)
            {
                case "L04":
                    {
                        mess = mess.Substring(2);
                        var mes_n_vint_u = 0;
                        int.TryParse(mess.Substring(0, 2), out mes_n_vint_u);
                        if (Station.n_vint_u != mes_n_vint_u)
                        {
                            Context.DiffLTR = true;
                            Station.n_vint_u = mes_n_vint_u;
                        }
                        var ordreVint = new int[34];
                        for (var i = 0; i < 34; i++)
                        {
                            ordreVint[i] = 0;
                        }
                        if (Station.n_vint_u < 34)
                        {
                            for (var i = 1; i <= Station.n_vint_u; i++)
                            {
                                var ordre = 0;
                                int.TryParse(mess.Substring(i * 2, 2), out ordre);
                                ordreVint[i] = ordre + 1; // BD.sta.OrdreVint(i) = Val(Mid(mess, 1 + i * 2, 2)) + 1
                            }
                        }
                        Station.OrdreVint = ordreVint;
                    }
                    break;
                case "L10":
                    {
                        mess = mess.Substring(2);
                        var n_enrg = 0;
                        int.TryParse(mess.Substring(0, 2), out n_enrg);
                        Station.n_enrg = n_enrg;
                        var cadScrut = 0;
                        int.TryParse(mess.Substring(2, 4), out  cadScrut);
                        Station.CadScrut = cadScrut;
                        var cadEnrg = 0;
                        int.TryParse(mess.Substring(6, 4), out cadEnrg);
                        Station.CadEnrg = cadEnrg;

                        for (var i = 0; i < 33; i++)
                        {
                            if (Station.VEnrg!.Count <= i)
                                Station.VEnrg.Add(new VoiesAnalogiques() { Num = 0 });
                            else
                                Station.VEnrg[i].Num = 0;
                        }
                        // Normalement, cette condition est toujours vraie
                        // mais on s'assure que c'est le cas
                        if (Station.n_enrg < Station.VEnrg!.Count)
                        {
                            for (var i = 1; i <= Station.n_enrg; i++)
                            {
                                var num = 0;
                                int.TryParse(mess.Substring(8 + i * 2, 2), out num);
                                Station.VEnrg[i].Num = num + 1; // BD.sta.VEnrg(i).num = Val(Mid(mess, 9 + i * 2, 2)) + 1
                            }
                        }
                    }
                    break;
                case "L14":
                    {
                        mess = mess.Substring(2);
                        var mess_n_et_u = 0;
                        int.TryParse(mess.Substring(0, 2), out mess_n_et_u);
                        if (Station.n_et_u != mess_n_et_u)
                        {
                            Context.DiffLTR = true;
                            Station.n_et_u = mess_n_et_u;
                        }
                        for (var i = 0; i < 17; i++)
                        {
                            if (Station.VEtat!.Count <= i)
                                Station.VEtat.Add(new VoiesEtat() { ETnum = 0 });
                            else
                                Station.VEtat[i].ETnum = 0;
                        }
                        // Normalement, cette condition est toujours vraie
                        // mais on s'assure que c'est le cas
                        if (Station.n_et_u < Station.VEtat!.Count)
                        {
                            for (var i = 1; i <= Station.n_et_u; i++)
                            {
                                var num = 0;
                                int.TryParse(mess.Substring(i * 2, 2), out num);
                                Station.VEtat[i].ETnum = num + 1; // BD.sta.VEtat(i).ETnum = Val(Mid(mess, 1 + i * 2, 2)) + 1
                            }
                        }
                    }
                    break;
                case "L23":
                    {
                        var nn = 0;
                        int.TryParse(mess.Substring(0, 2), out nn);
                        for (var i = 1; i <= nn; i++)
                        {
                            var index = (Station.OrdreVint != null && i < Station.OrdreVint.Length) ? Station.OrdreVint[i] : 0;
                            
                            if (index < Station.VInt!.Count)
                            {
                                var defaut = 0;
                                int.TryParse(mess.Substring((i - 1) * 9 + 2, 1), out defaut);
                                Station.VInt[index].Defaut = defaut;
                            }
                        }
                    }
                    break;
                case "L25":
                    {
                        mess = mess.Substring(2);
                        var pcMemoire = 0;
                        int.TryParse(mess.Substring(20, 2), out pcMemoire);
                        Station.PcMemoire = pcMemoire;

                        var jourCourant = 0;
                        int.TryParse(mess.Substring(22, 2), out jourCourant);
                        if (jourCourant > 63)
                        {
                            jourCourant = 0;
                        }
                        Context.JourCourant = jourCourant;

                        JourPrecedent = (Context.JourCourant + 63) % 64;

                        var jourBouclage = 0;
                        int.TryParse(mess.Substring(24, 2), out jourBouclage);
                        JourBouclage = jourBouclage;
                    }
                    break;
                case "L26":
                    {
                        mess = mess.Substring(2);
                        // 'mdf01_15  2/08/06   - pb année : 0006 au lieu de 2006
                        // 'BD.sta.DateDerTrfSta = CDate(mid(mess, 1, 2) + "/" + mid(mess, 3, 2) + "/" + mid(mess, 5, 4) + " " + mid(mess, 9, 2) + ":" + mid(mess, 11, 2) + ":00")
                        // BD.sta.DateDerTrfSta = CDate(Mid(mess, 1, 2) + "/" + Mid(mess, 3, 2) + "/" + Mid(mess, 7, 2) + " " + Mid(mess, 9, 2) + ":" + Mid(mess, 11, 2) + ":00")
                        var date = $"{mess.Substring(0, 2)}/{mess.Substring(2, 2)}/{mess.Substring(6, 2)} {mess.Substring(8, 2)}:{mess.Substring(10, 2)}:00";
                        var dateDerTrfSta = DateTime.MinValue;
                        DateTime.TryParse(date, out  dateDerTrfSta);
                        Station.DateDerTrfSta = dateDerTrfSta;
                    }
                    break;
                case "L29":
                    {
                        var nn = 0;
                        int.TryParse(mess.Substring(0, 2), out nn);
                        if (nn < Station.VEtat!.Count)
                        {
                            for (var i = 1; i <= nn; i++)
                            {
                                // BD.sta.VEtat(i).ETetat = (Mid(mess, i + 2, 1))
                                var etetat = 0;
                                int.TryParse(mess.Substring(i + 1, 1), out etetat);
                                Station.VEtat[i].ETetat = etetat;
                            }
                        }
                    }
                    break;
                case "L30":
                    {
                        if (Station.TableJours.Count >= 64)
                        {
                            for (var i = 0; i < 64; i++)
                            {
                                var valeur = $"{mess.Substring(i * 4 + 1)}/{mess.Substring(i * 4 + 4, 2)}";
                                Station.TableJours[i] = valeur;
                            }
                        }
                    }
                    break;
                case "L31":
                    {
                        if (Station.TableEvts.Count >= 64)
                        {
                            for (var i = 0; i < 64; i++)
                            {
                                var valeur = 0;
                                int.TryParse(mess.Substring(i * 4 + 2, 4), out valeur);
                                Station.TableEvts[i] = valeur;
                                int.TryParse(mess.Substring(0, 2), out var jprec);
                                if (i == Context.JourCourant)
                                {
                                    NbEvtsJourCourant = Station.TableEvts[i];
                                }
                                else if (jprec == JourPrecedent)
                                {
                                    NbEvtsJourPrecedent = Station.TableEvts[i];
                                }
                            }
                        }
                    }
                    break;
                case "L32":
                    {
                        if (Station.TableMots.Count >= 64)
                        {
                            for (var i = 0; i < 64; i++)
                            {
                                var valeur = 0x0;
                                var txtValeur = "0x1" + mess.Substring(i * 4 + 2, 4);
                                int.TryParse(txtValeur, out valeur);
                                Station.TableMots[i] = valeur % 65536;
                                int.TryParse(mess.Substring(0, 2), out var jprec);
                                if (i == Context.JourCourant)
                                {
                                    NbMotsJourCourant = Station.TableEvts[i];
                                }
                                else if (jprec == JourPrecedent)
                                {
                                    NbMotsJourCourant = Station.TableEvts[i];
                                }
                            }
                        }
                    }
                    break;
                case "L33":
                    {
                        int.TryParse(mess.Substring(0, 2), out var jcour);
                        int.TryParse(mess.Substring(2, 4), out var nbEvtsJour);
                        int.TryParse("0x1" + mess.Substring(6, 4), out var nbMotsJour);
                        if (Context.JourCourant == jcour)
                        {
                            NbEvtsJourCourant = nbEvtsJour;
                            NbMotsJourCourant = nbMotsJour % 65536;
                        }
                        else if (JourPrecedent == jcour)
                        {
                            NbEvtsJourPrecedent = nbEvtsJour;
                            NbMotsJourPrecedent = nbMotsJour % 65536;
                        }
                    }
                    break;
                case "L34":
                    {
                        var dh_sta = ToDate(mess);
                        Station.EcartHorloges = DateTime.Now.ToOADate() - dh_sta.ToOADate();
                        Logger.Log($"DateHeure Station: {dh_sta.ToString("dd/MM/yyyy HH:mm:ss")}; EcartHorloges: {Station.EcartHorloges}");
                    }
                    break;
                case "L35":
                    {
                        Station.DateMajSta = ToDate(mess);
                        Station.DefautDateParam = Station.DateMajSta != Station.DateMajBase;
                    }
                    break;
                case "L36":
                    {
                        mess = mess.Substring(2);
                        Station.Taches = mess.Substring(0, 16);
                        Station.VersionEnrg = mess.Substring(16, 11);
                        int.TryParse(Station.VersionEnrg.Substring(1, 2), out var versDialog);
                        Station.VersionDialog = versDialog;
                        Station.VersionEnregistrement = Station.VersionEnrg.Substring(3, 3);
                    }
                    break;
                case "L60":
                    {
                        Station.DateGo = ToDate(mess);
                        Logger.Log("DateGo: {0}", Station.DateGo.ToString("dd/MM/yyyy HH:mm:ss"));
                    }
                    break;
                case "L61":
                    {
                        Station.DateInit = ToDate(mess);
                        Logger.Log("DateInit: {0}", Station.DateInit.ToString("dd/MM/yyyy HH:mm:ss"));
                    }
                    break;
                case "L62":
                    {
                        Station.DateAcq = ToDate(mess);
                        Logger.Log("DateAcq: {0}", Station.DateAcq.ToString("dd/MM/yyyy HH:mm:ss"));
                    }
                    break;
                case "L63":
                    {
                        Station.DateEnrg = ToDate(mess);
                        Logger.Log("DateEnrg: {0}", Station.DateEnrg.ToString("dd/MM/yyyy HH:mm:ss"));
                    }
                    break;
                case "L64":
                    {
                        Station.DateStop = ToDate(mess);
                        Logger.Log("DateStop: {0}", Station.DateStop.ToString("dd/MM/yyyy HH:mm:ss"));
                    }
                    break;
                case "":
                    message = $"Erreur: message non reconnu - {message}";
                    break;
                default:
                    break;
            }

            message = $"{message} ; {texteMessage}";
            message = message.Replace("'", "_").Replace('"', '_');

            return message;
        }

        private DateTime ToDate(string ms)
        {
            ms = ms.Substring(2);
            var strDate = $"{ms.Substring(0, 2)}/{ms.Substring(2, 2)}/{ms.Substring(6, 2)} {ms.Substring(8, 2)}:{ms.Substring(10, 2)}:{ms.Substring(12, 2)}";
            if (!DateTime.TryParse(strDate, out var date))
            {
                throw new Exception($"Error converting '{strDate}' to DateTime");
            }
            if (date < new DateTime(2000, 1, 1))
            {
                throw new Exception($"Erreur de date : '{date.ToString("yyyy/MM/dd HH:mm:ss")}'");
            }
            return date;
        }
        #endregion
    }
}

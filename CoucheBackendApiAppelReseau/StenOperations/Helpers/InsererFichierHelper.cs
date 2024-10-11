
using Microsoft.VisualBasic;
using StenOperations.Logger;
using StenOperations.Utils;


namespace StenOperations.Helpers
{
    /// <summary>
    /// Insertion de valeurs dans le fichier semaine
    /// </summary>
    public class InsererFichierHelper
    {
        private string lgSrc, lgSem;
        private DateTime DateSem { get; set; }
        private DateTime DateSrc { get; set; }

        private bool flagPasErrDate;

        private string mFicSem = "";
        private string mFicSrc = "";


        private long posTmpCourante = 0;
        private const int ERRDEBUTSEM = 20;
        private const int ERRDEBUTSRC = 22;

        private string evtSem, evtSrc;
        private bool flagFSrc, flagFSem, etoile; 
        private bool finFicSrc, finFicSem;
        private string[] tableauTmp = new string[65536];

        private FileStream fsSrc;
        private FileStream fsSem;
        private StreamReader srFicSrc = null;
        private StreamReader srFicSem = null;

        public DateTime DateSuite { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorText { get; set; } = "";
        public ILogger Logger { get; set; }

        public InsererFichierHelper(ILogger logger)
        {
            Logger = logger;
        }

        public int ErrConcatenation(string nomFicMan)
        {
            //if (!FichierSourceOk(nomFicMan))
            //{
            //    posTmpCourante = -1;
            //    //procQuitter(ERRDEBUTSRC);
            //    return 1;
            //}

            return 0;
        }


        /// <summary>
        /// Vérifie si le nom du fichier est correct et initialise mFicSrc
        /// </summary>
        /// <param name="nomFicMan"></param>
        /// <returns></returns>
        private bool FichierSourceOk(string nomFicMan) 
        {
            var mficSrc = nomFicMan;
            var fi = new FileInfo(nomFicMan);
            var fic = fi.Name;
            int.TryParse(fic.Substring(0, 3), out var res0);
            var pFirst = fic.Substring(0, 1).ToUpper();
            int.TryParse(fic.Substring(1, 2), out var res2);
            if (res0 > 0) 
            {
                fic = fic.Substring(4, 5); // resobs
            }
            else if (pFirst == "P" && res2 > 0) 
            {
                fic = fic.Substring(4, 5); // resobs
            }
            else
            {
                fic = fic.Substring(3, 5);
            }
            var jj = fic.Substring(0, 2);
            var m = fic.Substring(2, 1);
            var aa = fic.Substring(3, 2);
            int.TryParse(aa, out var annee);
            if (annee > 95)
            {
                aa = $"19{aa}";
            }
            else
            {
                aa = $"20{aa}";
            }
            var ints = Enumerable.Range(1, 9).Select(r => r.ToString()).ToList();
            var ond = new List<string>() { "O", "N", "D" };
            if (ints.Contains(m))
            { }
            else if (ond.Contains(m.ToUpper()))
            {
                m = (m.ToUpper() == "O") ? "10" : (m.ToUpper() == "N" ? "11" : "12");
            }
            else
            {
                m = "0";
            }
            int.TryParse(aa, out annee);
            int.TryParse(m, out var month);
            int.TryParse(jj, out var day);
            var d = DateAndTime.DateSerial(annee, month, day);

            return TrouverFicSem(d, mficSrc);
        }

        private bool TrouverFicSem(DateTime dx, string mFicSrc)
        {
            // V260 modifs des initiales station: 2 lettres ou 3 chiffres (resobs)
            var fi = new FileInfo(mFicSrc);
            var fSem = fi.Name;
            var repSta = "";
            var mPathSem = "";

            int.TryParse(fSem.Substring(0, 3), out var res0);
            var pFirst = fSem.Substring(0, 1).ToUpper();
            int.TryParse(fSem.Substring(1, 2), out var res1);
            if (res0 > 0)
            {
                repSta = $"{mPathSem}{fSem.Substring(0, 3)}\\";
                fSem = fSem.Substring(0, 4);
            }
            else if (pFirst == "P" && res1 > 0)
            {
                repSta = $"{mPathSem}{fSem.Substring(0, 3)}\\";
                fSem = fSem.Substring(0, 4);
            }
            else
            {
                repSta = $"{mPathSem}{fSem.Substring(0, 2)}\\";
                fSem = fSem.Substring(0, 3);
            }

            var j = DateAndTime.Weekday(dx);
            j = (j == 1) ? 6 : j - 2;
            dx = dx.AddDays(-j);

            if (DateAndTime.Month(dx) < 10)
            {
                fSem = $"{fSem}{dx.ToString("ddMyy")}";
            }
            else if (DateAndTime.Month(dx) == 10)
            {
                fSem = $"{fSem}{dx.ToString("dd\\Oyy")}";
            }
            else if (DateAndTime.Month(dx) == 11)
            {
                fSem = $"{fSem}{dx.ToString("dd\\Nyy")}";
            }
            else if (DateAndTime.Month(dx) == 12)
            {
                fSem = $"{fSem}{dx.ToString("dd\\Dyy")}";
            }
            var date = new DateTime(dx.Year, 01, 01);
            j = DateAndTime.Weekday(date);
            j = (j == 4 || j == 5) ? 2 : 1;

            fSem = $"{fSem}{((int)(dx.DayOfYear / 7) + j)}.S00";

            DateSem = dx;

            mFicSem = $"{repSta}{fSem}";
            if (!Directory.Exists(repSta))
            {
                try
                {
                    Directory.CreateDirectory(repSta);
                }
                catch (Exception ex)
                {
                    ErrorCode = 23;
                    ErrorText = "Défaut création répértoire semaine. " + ex.ExceptionMessages();
                    return false;
                }
            }

            return true;
        }

        private void procQuitter(int etat)
        {
            if (posTmpCourante >= 0)
            {
                var bakMFicSem = $"{mFicSem}.bak";
                if (File.Exists(bakMFicSem))
                {
                    File.Delete(bakMFicSem); 
                }
                File.Copy(mFicSem, bakMFicSem);

                using (var fsTmp = new FileStream(mFicSem, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var swFicTmp = new StreamWriter(fsTmp))
                    {
                        if (!string.IsNullOrEmpty(tableauTmp[0]))
                        {
                            swFicTmp.WriteLine(tableauTmp[0]);
                        }
                        for (var i = 1; i <= posTmpCourante; i++)
                        {
                            if (!string.IsNullOrEmpty(tableauTmp[i]) &&
                                tableauTmp[i] != tableauTmp[i - 1])
                            {
                                swFicTmp.WriteLine(tableauTmp[i]);
                            }
                        }
                    }
                }
                File.Delete(bakMFicSem);
            }

            var dirFicErr = ""; // vgDirCrAppel
            var fi = new FileInfo(mFicSrc);
            int.TryParse(fi.Name.Substring(0, 3), out var errDir);
            if (errDir > 0) 
            {
                dirFicErr += $"{fi.Name.Substring(0, 3)}\\";
            }
            else
            {
                dirFicErr += $"{fi.Name.Substring(0, 2)}\\";
            }
            var ficerr = $"{dirFicErr}{fi.Name}_err{DateTime.Now.ToString("ddMMyyyyHHmmss")}.txt";

            switch(etat)
            {
                case 0:
                    {
                        ErrorText = "";
                        DateSuite = DateSrc;
                        if (File.Exists(mFicSrc))
                        {
                            File.Delete(mFicSrc);
                        }
                    }
                    break;
                case 35:
                    {
                        ErrorText = "Fin fichier Src atteinte avec *";
                        DateSuite = DateSrc;
                        if (File.Exists(mFicSrc))
                        {
                            File.Delete(mFicSrc);
                        }
                    }
                    break;
                case 25:
                    {
                        ErrorText = "Défaut fichier reçu - erreur rattrapable (25)";
                        DateSuite = DateSrc;
                        if (File.Exists(mFicSrc))
                        { 
                            File.Copy(mFicSrc, ficerr);
                            File.Delete(mFicSrc);
                        }
                    }
                    break;
                case ERRDEBUTSEM:
                    {
                        ErrorText = "Erreur fichier semaine";
                        if (File.Exists(mFicSrc))
                        {
                            File.Copy(mFicSrc, ficerr);
                            File.Delete(mFicSrc);
                        }
                        if (File.Exists(mFicSem))
                        {
                            var fis = new FileInfo(mFicSem);
                            File.Copy(mFicSem, $"{dirFicErr}{fis.Name}_err{DateTime.Now.ToString("ddMMyyyyHHmmss")}.txt");
                            File.Delete(mFicSem);
                        }
                    }
                    break;
                case ERRDEBUTSRC:
                    {
                        ErrorText = "Erreur début ou nom fichier reçu ";
                        if (File.Exists(mFicSrc))
                        {
                            File.Copy(mFicSrc, ficerr);
                            File.Delete(mFicSrc);
                        }
                    }
                    break;
                default:
                    break;
            }
        }



        private bool lectSem()
        {
            if (srFicSem == null)
                return false;
            lgSem = srFicSem.ReadLine().TrimEnd();
            return DHlgSem();
        }

        private bool lectSrc()
        {
            // ce qui n'arrive jamais
            if (srFicSrc == null)
                return false;
            var resp = false;
            do
            {
                lgSrc = srFicSrc.ReadLine().TrimEnd();
                resp = DHlgSrc();
            }
            while (lgSrc == "" && !finFicSrc);
            
            return resp;
        }

        private bool DHlgSem()
        {
            var xl = "";
            evtSem = lgSem.Substring(0, 1);
            switch(evtSem)
            {
                case "#":
                    break;
                case "D":
                    {
                        var date = lgSem.Substring(1, lgSem.IndexOf(" ") - 1);
                        var hour = lgSem.Substring(lgSem.IndexOf("h") - 1, 5).Replace("h", ":");
                        xl = $"{date} {hour}:00";
                        DateTime.TryParse(xl, out var dxl);
                        DateSem = dxl;
                    }
                    break;
                case "E":
                    {
                        lgSem = lgSem.Substring(0, 13) + "0000";
                        xl = lgSem.Substring(1, lgSem.IndexOf(" ") - 1);
                        DateTime.TryParse(xl, out var dxl);
                        DateSem = dxl;
                    }
                    break;
                case "F":
                    {
                        var date = lgSem.Substring(1, lgSem.IndexOf(" ") - 1);
                        var hour = lgSem.Substring(lgSem.IndexOf("h") - 1, 5).Replace("h", ":");
                        xl = $"{date} {hour}:00";
                        DateTime.TryParse(xl, out var dxl);
                        DateSem = dxl;
                        flagFSem = true;
                    }
                    break;
                default:
                    {
                        xl = lgSem.Substring(1, 6).Replace("h", ":");
                        if (xl.Length >= 6)
                        {
                            DateTime.TryParse($"{DateSem.ToString("dd/MM/yyyy")} {xl}", out var dtxl);
                            DateSem = dtxl;
                        }
                    }
                    break;
            } // switch(evtSem)
            return false;
        }

        private bool DHlgSrc()
        {
            var xl = "";
            DateTime dh = DateTime.MinValue;
            bool errDate = false;
            evtSrc = lgSrc.Substring(0, 1);
            switch (evtSrc)
            {
                case "#":
                    finFicSrc = false;
                    break;
                case "D":
                    { 
                        if (lgSrc.Length != 23) { ErrorCode = 5; }
                        var date = lgSrc.Substring(1, lgSrc.IndexOf(" ") - 1);
                        var hour = lgSrc.Substring(lgSrc.IndexOf("h") - 1, 5);
                        xl = $"{date} {hour}".Replace("h", ":");
                        xl += ":00"; 
                        DateTime.TryParse(xl, out dh);
                    }
                    break;
                case "E":
                    {
                        lgSrc = $"{lgSrc.Substring(0, 13)}0000";
                        xl = lgSrc.Substring(1, lgSrc.IndexOf(" ") - 1);
                        DateTime.TryParse(xl, out dh);
                        var dsrc = DateTime.Parse(DateSrc.ToString("dd/MM/yyyy"));
                        errDate = (dh < dsrc);
                    }
                    break;
                case "F":
                    {
                        if (lgSrc.Length != 18) { ErrorCode = 5; }
                        var date = lgSrc.Substring(1, lgSrc.IndexOf(" ") - 1);
                        var hour = lgSrc.Substring(lgSrc.IndexOf("h") - 1, 5);
                        xl = $"{date} {hour}".Replace("h", ":");
                        xl += ":00";
                        DateTime.TryParse(xl, out dh);
                        flagFSrc = true;
                    }
                    break;
                case "H":
                    {
                        if (lgSrc.Length < 6) { ErrorCode = 5; }
                        xl = lgSrc.Replace("h", ":");
                        xl = xl.Substring(1, 5);
                        DateTime.TryParse($"{DateSrc.ToString("dd/MM/yyyy")} {xl}:00", out dh);
                    }
                    break;
                default:
                    {
                        if (lgSrc == "")
                        {
                            if (srFicSrc.Peek() == -1) { finFicSrc = true; }
                        }
                        else if ((lgSrc.Substring(0, 6) == "M23h59") || (lgSrc.Substring(0, 6) == "B23h59"))
                        {
                            evtSrc = "";
                            lgSrc = "";
                        }
                        else
                        {
                            if (lgSrc.Length < 6) { ErrorCode = 5; }
                            xl = lgSrc.Replace ("h", ":");
                            xl = xl.Substring(1, 5);
                            DateTime.TryParse($"{DateSrc.ToString("dd/MM/yyyy")} {xl}:00", out dh);
                            errDate = (dh < DateSrc) && (!flagPasErrDate);
                            if (errDate && (evtSrc == "G" || evtSrc == "P" || evtSrc == "T"))
                            {
                                errDate = false;
                                evtSrc = "";
                                lgSrc = "";
                            }
                        }
                    }
                    break;
            } // switch (evtSrc)

            if (dh.Ticks != 0 && !errDate) { DateSrc = dh; } // si pas erreur date
            etoile = lgSrc.IndexOf("*") != -1;
            finFicSrc = finFicSrc || (ErrorCode != 0) || etoile || flagFSrc || errDate;

            return false;
        }


    }
}

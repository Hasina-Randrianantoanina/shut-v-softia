using CoucheLectureMan.Entities;
using CoucheLectureMan.Entities.Infos;

namespace CoucheLectureMan.Utils
{
    public static class AutomateManFileUtils
    {
        public static List<VoieInterne> GetAnalogicConfigFromManFile(Station station, string[] lines)
        {
            List<VoieInterne> unexpectedVoiesInternes = new List<VoieInterne>();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] wordsOfALine;
                string fistWord;
                string secondWord;
                string thirdWord;

                switch (line[0])
                {
                    case '#':
                        // Voie interne, split by tab
                        wordsOfALine = lines[i].Split('\t');
                        fistWord = wordsOfALine[0];
                        secondWord = wordsOfALine[1];
                        thirdWord = wordsOfALine[2];

                        VoieInterne voieInterneRead = new VoieInterne
                        {
                            Libelle = secondWord,
                            NumeroVoie = GetNumeroVoie(fistWord[1]),
                            Virgule = int.Parse(thirdWord[1].ToString())
                        };
                        VoieInterne voieInterneExpected = station.VoiesInternes.Find((ve) => { return ve.NumeroVoie == voieInterneRead.NumeroVoie; });

                        if (voieInterneExpected == null || voieInterneRead.Libelle.Equals(voieInterneExpected.Libelle) == false
                            || voieInterneRead.Virgule != voieInterneExpected.Virgule)
                        {
                            unexpectedVoiesInternes.Add(voieInterneRead);
                        }
                        break;
                    case 'D':
                        // Line D, split by double space
                        wordsOfALine = lines[i].Split("  ");
                        thirdWord = wordsOfALine[2];
                        int numeroStation = Convert.ToInt32(thirdWord);
                        if (station.Numero != numeroStation) Console.WriteLine($"\n--- NumeroStation incorrect : found {numeroStation}, expected {station.Numero}");
                        break;
                    case 'E':
                        // Ligne E, split by space
                        wordsOfALine = lines[i].Split("  ");
                        secondWord = wordsOfALine[1];
                        Console.WriteLine($"Number of events = {secondWord}");
                        break;
                    default:
                        break;

                }
                return unexpectedVoiesInternes;
            }
            return unexpectedVoiesInternes;
        }

        public static List<Mesure> GetMesuresFromManFile(Station station, string[] lines)
        {
            List<Mesure> mesures = new List<Mesure>();
            string day = "";
            bool enregistrementAMinuit = true;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line[0] == '#' || line[0] == 'D')
                {
                    // Config Ana, Line D
                    continue;
                }
                else if (line[0] == 'E')
                {
                    // Line E
                    day = line.Substring(1, 10);
                    continue;
                }
                else if (line[0] == 'F')
                {
                    // End of the file
                    break;
                }

                string[] wordsOfALine = lines[i].Split(' ');
                string firstWord = wordsOfALine[0];
                string secondWord = wordsOfALine[1];
                string thirdWord;
                DateTime mesureTime;

                if (firstWord.Equals(""))
                {
                    // Case Analogic mesure with positiv value, first char is space
                    mesureTime = DateTime.Parse($"{day} {secondWord.Substring(0, 2)}:{secondWord.Substring(3, 2)}:00");
                }
                else
                {
                    mesureTime = DateTime.Parse($"{day} {firstWord.Substring(1, 2)}:{firstWord.Substring(4, 2)}:00");
                }

                if (mesureTime.Hour != 0  || mesureTime.Minute != 0) enregistrementAMinuit = false;

                Mesure mesure;
                List<Mesure> mesuresOnThisLine;

                switch (line[0])
                {
                    case 'J':
                        // Analogic mesure (CAL, CAP) with positiv value & EnregistrementAMinuit
                        mesuresOnThisLine = GetMesuresOfThisLine(line, mesureTime, true, true);
                        mesures.AddRange(mesuresOnThisLine);
                        break;
                    case ' ':
                        // Analogic mesure (CAL, CAP) with positiv value
                        line = line.Trim();
                        mesuresOnThisLine = GetMesuresOfThisLine(line, mesureTime, true, false);
                        mesures.AddRange(mesuresOnThisLine);
                        break;
                    case '-':
                        // Analogic mesure (CAL, CAP) with negativ value
                        mesuresOnThisLine = GetMesuresOfThisLine(line, mesureTime, false, enregistrementAMinuit);
                        mesures.AddRange(mesuresOnThisLine);
                        break;
                    case 'k':
                        // Mesure ECH 0%
                        mesure = new Mesure
                        {
                            Time = mesureTime,
                            NumeroVoie = GetNumeroVoie(secondWord[0]),
                            ValeurALEchelle = int.Parse(secondWord.Substring(1, 4)),
                            Info = Info.GetInfoOfThisMesure("ECH", GetInfoValue("ECH", line, false))
                        };
                        mesure.Info.TypeMesure = "ECH0%";

                        mesures.Add(mesure);
                        break;
                    case 'm':
                        mesure = new Mesure
                        {
                            Time = mesureTime,
                            NumeroVoie = GetNumeroVoie(secondWord[0]),
                            ValeurALEchelle = int.Parse(secondWord.Substring(1, 4)),
                            Info = Info.GetInfoOfThisMesure("ECH", GetInfoValue("ECH", line, false))
                        };
                        mesure.Info.TypeMesure = "ECH100%";

                        mesures.Add(mesure);
                        break;
                    case 'r':
                        // Mesure TS & EnregistrementAMinuit
                        thirdWord = wordsOfALine[2];

                        mesure = new Mesure
                        {
                            Time = mesureTime,
                            NumeroVoie = int.Parse(secondWord.Substring(1, 4)),
                            ValeurALEchelle = int.Parse(thirdWord.Substring(4, 1)),
                            Info = Info.GetInfoOfThisMesure("TS", GetInfoValue("TS", line, true))
                        };

                        mesures.Add(mesure);
                        break;
                    case 's':
                        // Mesure TS
                        thirdWord = wordsOfALine[2];

                        mesure = new Mesure
                        {
                            Time = mesureTime,
                            NumeroVoie = int.Parse(secondWord.Substring(1, 4)),
                            ValeurALEchelle = int.Parse(thirdWord.Substring(4, 1)),
                            Info = Info.GetInfoOfThisMesure("TS", GetInfoValue("TS", line, false))
                        };

                        mesures.Add(mesure);
                        break;
                    case 'o':
                        // Mesure TS & EnregistrementAMinuit
                        thirdWord = wordsOfALine[2];

                        mesure = new Mesure
                        {
                            Time = mesureTime,
                            NumeroVoie = int.Parse(secondWord.Substring(1, 4)),
                            ValeurALEchelle = int.Parse(thirdWord.Substring(4, 1)),
                            Info = Info.GetInfoOfThisMesure("TC", GetInfoValue("TC", line, true))
                        };

                        mesures.Add(mesure);
                        break;
                    case 'c':
                        // Mesure TC 
                        thirdWord = wordsOfALine[2];

                        mesure = new Mesure
                        {
                            Time = mesureTime,
                            NumeroVoie = int.Parse(secondWord.Substring(1, 4)),
                            ValeurALEchelle = int.Parse(thirdWord.Substring(4, 1)),
                            Info = Info.GetInfoOfThisMesure("TC", GetInfoValue("TC", line, false))
                        };

                        mesures.Add(mesure);
                        break;
                    default:
                        Console.WriteLine($"\n--- Error when reading the line : {line}");
                        continue;
                }
            }

            return mesures;
        }

        public static List<Mesure> GetMesuresOfThisLine(string line, DateTime mesureTime, bool positivValue, bool enregistrementAMinuit)
        {
            List<Mesure> mesuresOnThisLine = new List<Mesure>();
            string[] wordsOfALine = line.Split(' ');

            for (int i = 1; i < wordsOfALine.Length; i++)
            {
                string word = wordsOfALine[i]; // aXXXX

                Mesure mesure = new Mesure
                {
                    Time = mesureTime,
                    NumeroVoie = GetNumeroVoie(word[0]),
                    ValeurALEchelle = int.Parse(word.Substring(1, 4)),
                    Info = Info.GetInfoOfThisMesure("CAP", GetInfoValue("CAP", line, enregistrementAMinuit)) // From the man file we can't know if it's a CAP or CAL value
                };

                if (positivValue == false) mesure.ValeurALEchelle *= -1;
                mesuresOnThisLine.Add(mesure);
            }

            return mesuresOnThisLine;
        }

        public static void UpdateValeurALEchelle(List<VoieInterne> voiesInternes, List<Mesure> mesures)
        {
            foreach (Mesure mesure in mesures)
            {
                if (mesure.Info.TypeMesure.Equals("TS") || mesure.Info.TypeMesure.Equals("TC")) continue;

                int virgule = 0;
                foreach (VoieInterne voieInterne in voiesInternes)
                {
                    if (voieInterne.NumeroVoie == mesure.NumeroVoie)
                    {
                        virgule = voieInterne.Virgule;
                        break;
                    }
                }

                mesure.ValeurALEchelle = Convert.ToSingle(mesure.ValeurALEchelle/Math.Pow(10, virgule));
            }
        }

        private static int GetNumeroVoie(char codeVoie)
        {
            char[] lastCodes = { '(', ')', '{', '}', '[', ']' };

            // a to z => 1 to 26
            if (codeVoie > 96  && codeVoie < 96+26+1)
            {
                return codeVoie - 96;
            }

            // ( to ] => 27 to 32
            for (int i = 0; i < lastCodes.Length; i++)
            {
                if (lastCodes[i] == codeVoie) return i + 26;
            }

            Console.WriteLine("\n--- Error when reading codeVoie");
            return -1;
        }

        private static Int16 GetInfoValue(string typeMesure, string line, bool enregistrementAMinuit)
        {
            Int16 value = 0;
            string thirdWord;
            switch (typeMesure)
            {
                case "CAP":
                    value = ConversionUtils.GetIntegerValueOfThisBit(14); // From the man file we can't know if it's a CAP or CAL value
                    if (enregistrementAMinuit == false)
                    {
                        // From the man file we can't know if it's a DepassementDelta or DepassementSeuil
                        value += ConversionUtils.GetIntegerValueOfThisBit(0);
                    }
                    break;
                case "TC":
                    value =  ConversionUtils.GetIntegerValueOfThisBit(13);
                    thirdWord = line.Split(' ')[2];
                    if (thirdWord.Substring(1, 2) == "21") value += ConversionUtils.GetIntegerValueOfThisBit(0); // ApparitionDefautBattement
                    if (thirdWord.Substring(1, 2) == "20") value += ConversionUtils.GetIntegerValueOfThisBit(1); // DisparitionDefautBattement
                    break;
                case "TS":
                    value =  ConversionUtils.GetIntegerValueOfThisBit(12);
                    thirdWord = line.Split(' ')[2];
                    if (thirdWord.Substring(1, 2) == "21") value += ConversionUtils.GetIntegerValueOfThisBit(0); // ApparitionDefautBattement
                    if (thirdWord.Substring(1, 2) == "20") value += ConversionUtils.GetIntegerValueOfThisBit(1); // DisparitionDefautBattement
                    break;
                case "ECH":
                    value =  ConversionUtils.GetIntegerValueOfThisBit(11);
                    // NB : EnregistrementAMinuit not used in the writing of the man file
                    break;
            }

            if (enregistrementAMinuit) value += ConversionUtils.GetIntegerValueOfThisBit(2);

            return value;
        }
    }
}

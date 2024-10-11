using CoucheTraitementDonnees.AutomateProtocol;
using CoucheTraitementDonnees.Entities;
using CoucheTraitementDonnees.Utils;
using System.Diagnostics;

namespace CoucheTraitementDonnees
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // -------------- Choose the initiales -----------------

            // M580 => GA (D15 RU) or HE (D16 RU) or 172 (D15 RO) or 178 (D15 RO with Mesure ECH)
            // Premium => BP ("" RU) or EN (D13 RU) or RC ("" RU, defauts)
            // Load Test Premium => LTP
            string initiales = "EN";

            // ------ Do not change anything after this ------------
            TimeSpan timeToReadDatas;
            TimeSpan timeToWriteMan;

            Station station = DataBaseUtils.GetStation(initiales);
            if (station == null) return;

            // Input files
            string[] sourceFilesFullNames = FileUtils.GetSourceFilesFullNames(station.Initiales);
            if (sourceFilesFullNames == null) return;

            // Output files
            string binaryFileDirectory = $"BinaryFiles/{station.Initiales}/";

            // 1 binary file = 1 man file
            for (int i = 0; i < sourceFilesFullNames.Length; i++)
            {
                DateTime today = DateTime.Now;
                today = GetDate(station.Initiales, i); // Dev mode
                string binaryFileFullName = binaryFileDirectory + $"{station.Initiales}_{today.ToString("ddMMyyyyHHmmss")}.bin";

                // Step n°1 : copy the file
                bool copySuccessfull = FileUtils.TryCopyFile(sourceFilesFullNames[i], binaryFileFullName);
                if (copySuccessfull == false) return;

                // ----------------------- ISODAQ ----------------------
                if (station.TypeLiaison == "?")
                {
                    // Step n°2 : reading the datas in the binary file
                    // TODO : ISODAQProtocol

                }

                // --------------------- STEN --------------------------
                else if (station.TypeLiaison == "IP")
                {
                    // Step n°2 : reading the datas in the binary file
                    // TODO : STENProtocol

                }

                // --------------------- Automate ----------------------
                else if (station.TypeLiaison == "AP")
                {
                    // Step n°2 : reading the datas in the binary file
                    Stopwatch clock = Stopwatch.StartNew();

                    // Instanciate Specific Automate Protocol
                    ISpecificAutomateProtocol specificAutomateProtocol = ISpecificAutomateProtocol.GetSpecificAutomateProtocol(station);
                    if (specificAutomateProtocol == null) return;

                    bool premiumProtocol = specificAutomateProtocol is AutomatePremiumProtocol;
                    specificAutomateProtocol = premiumProtocol ? (AutomatePremiumProtocol)specificAutomateProtocol : (AutomateM580Protocol)specificAutomateProtocol;

                    // Creating the list of mesures of this file
                    List<Mesure> mesures = specificAutomateProtocol.GetMesuresFromBinaryFile(binaryFileFullName, station.Version);

                    clock.Stop();
                    timeToReadDatas = clock.Elapsed;

                    // Deleting mesures showing date inversions
                    mesures = AutomateManFileUtils.DeleteDateInversions(mesures, true);

                    PrintUtils.PrintMesures(mesures, false);

                    station.Mesures.AddRange(mesures); // Could be useless if we insert the datas in the DB each loop

                    // Step n°3 : writing the data in .man file
                    clock = Stopwatch.StartNew();

                    bool manFileWritingSuccessfull = await specificAutomateProtocol.TryWriteManFile(station, binaryFileFullName, mesures); // Using mesures and not station.Mesures because there could be mesures from other binary files
                    if (manFileWritingSuccessfull == false) return;

                    clock.Stop();
                    timeToWriteMan = clock.Elapsed;

                    // Step n°4 : append the man file to the weekly file
                    // TODO

                    // Step n°5 : writing the data in the DataBase ?
                    // TODO

                    // Step n°6 : handling the Defauts
                    // TODO

                    Console.WriteLine($"\nTime spend reading datas : {timeToReadDatas}");
                    Console.WriteLine($"Time spend writing the man : {timeToWriteMan}");
                }

                // ---------------------- Unknown ----------------------
                else
                {
                    Console.WriteLine("\n---- Protocol Unknown ----\n");
                }
            } // End for

        } // End Main

        // Temp
        private static DateTime GetDate(string initiales, int loopCount)
        {
            DateTime date = DateTime.Now;

            // Dev mode
            switch (initiales)
            {
                case "BP": // Premium "" RU
                    date = DateTime.Parse("20/01/2022 14:20:18");
                    break;
                case "EN": // Premium D13 RU
                    date = DateTime.Parse("13/05/2024 11:20:06");
                    break;
                case "GA": // M580 D15 RU
                    date = (loopCount == 1) ? DateTime.Parse("01/01/2022 02:20:56") : DateTime.Parse("01/01/2022 02:20:58");
                    break;
                case "HE": // M580 D16 RU
                    date = DateTime.Parse("01/01/2022 05:20:55");
                    break;
                case "RC": // Premium "" RU with Defauts
                    date = DateTime.Parse("13/01/2022 07:20:56");
                    break;
                case "172": // M580 D15 RO
                    date = DateTime.Parse("28/12/2021 16:20:07");
                    break;
                case "178": // M580 D15 RO with Mesure ECH
                    date = DateTime.Parse("29/12/2021 22:20:17");
                    break;
            }

            return date;
        }

    }
}

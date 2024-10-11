using CoucheTraitementDonnees.Entities;
using CoucheTraitementDonnees.Utils;

namespace CoucheTraitementDonnees.AutomateProtocol
{
    public class AutomateM580Protocol : ISpecificAutomateProtocol
    {
        public List<Mesure> GetMesuresFromBinaryFile(string binaryFileFullName, string versionEnregistreur)
        {
            if (File.Exists(binaryFileFullName) == false)
            {
                Console.WriteLine($"\n--- {binaryFileFullName} is missing, no reading done");
                return new List<Mesure>();
            }

            Console.WriteLine($"\n------ Starting to read the binary file {binaryFileFullName}...");

            switch (versionEnregistreur)
            {
                case "D15A55Ca01": // 4 lines per Mesure
                    return AutomateBinaryFileUtils.ReadAllMesuresM580(binaryFileFullName, 4);
                case "D16A56Ca01": // 5 lines per Mesure
                    return AutomateBinaryFileUtils.ReadAllMesuresM580(binaryFileFullName, 5);
                default:
                    return new List<Mesure>();
            }

        }

        public async Task<bool> TryWriteManFile(Station station, string binaryFileFullName, List<Mesure> mesures)
        {
            // Output files
            string manFileDirectory = $"ManFiles/{station.Initiales}/";
            string manFileName = Path.GetFileName(binaryFileFullName).Replace(".bin", ".man");
            string manFileFullName = $"{manFileDirectory}{manFileName}";

            if (mesures.Count == 0)
            {
                Console.WriteLine($"\n--- Creating of the man file {manFileName} aborted : there are no mesures to write");
                return false;
            }

            Console.WriteLine($"\n------ Creating the man file {manFileName}");

            // Creating the .man
            if (FileUtils.TryDeleteCreateFile(manFileFullName))
            {
                Console.WriteLine($"\nSuccessfully created {manFileName} in {manFileDirectory}");

                // Step 1 : writing the header
                bool headerSucessfullWrite = await AutomateManFileUtils.TryWriteHeader(manFileFullName, station, mesures); // Using mesures and not station.Mesures because there could be mesures from other binary files

                // Step 2 : writing the Mesures
                bool mesuresSucessfullWrite = await AutomateManFileUtils.TryWriteMesures(manFileFullName, station, mesures); // Using mesures and not station.Mesures because there could be mesures from other binary files

                // Step 3 : writing the last line
                bool lastLineSucessfullWrite = await AutomateManFileUtils.TryWriteLastLine(manFileFullName, mesures); // Using mesures and not station.Mesures because there could be mesures from other binary files


                if (headerSucessfullWrite && mesuresSucessfullWrite && lastLineSucessfullWrite)
                {
                    Console.WriteLine($"\n------ End of writing : successfully wrote {manFileName}");
                    return true;
                }
                else
                {
                    string errors = "";
                    if (headerSucessfullWrite == false) errors += " Writing of Header";
                    if (mesuresSucessfullWrite == false) errors += " Writing of Mesures";
                    if (lastLineSucessfullWrite == false) errors += " Writing of Last line";
                    Console.WriteLine($"\n------ End of writing : errors during writing{errors}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"\n--- Error when deleting and creating the file {manFileFullName}");
                return false;
            }
        }
    }
}

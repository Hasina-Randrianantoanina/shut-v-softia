using IsodaqOperations.Entities;
using System.Xml;

namespace IsodaqOperations.Utils
{
    public static class ISODAQUtils
    {
        public static string GetEnregistreurVersion(XmlNode document)
        {
            string? eserno = XMLUtils.GetInnerText(document, @"/xdq/system/eserno");
            string? appcard = XMLUtils.GetInnerText(document, @"/xdq/system/appcard") == null ? "--" : XMLUtils.GetInnerText(document, @"/xdq/system/appcard");
            string? fversion = XMLUtils.GetInnerText(document, @"/xdq/system/fversion");
            return $"{eserno}/{appcard}/{fversion}";
        }

        public static (double, string) GetBatteryInfo(XmlNode document)
        {
            const double RT = 3.41;
            const double Sigma = 12.2;
            string? infoBatterie = XMLUtils.GetInnerText(document, @"/xdq/system/batt") == null ? XMLUtils.GetInnerText(document, @"/xdq/batt") : XMLUtils.GetInnerText(document, @" / xdq/system/appcard");
            if (infoBatterie == null ) { return (0, string.Empty); }
            var parse = infoBatterie.Split(" ");
            if (parse.Length > 1)
            {
                return (Sigma, parse[1]);
            }
            else
            {
                return (RT, parse[0]);
            }
        }

        public static List<Mesure>? GetMesures(XmlNode voieXML, VoieInterne voieInterne, bool printConsole)
        {
            List<Mesure>? mesuresOfThisVoie;

            int[]? xAxis = null;
            double[]? yAxis = null;
            DateTime? startingDate;
            int measuringStep;
            int precision;
            string[]? rawDatas;

            try
            {
                // First step : Get reference table
                xAxis = GetReferenceTableXAxis(voieXML); // X axis reference table
                yAxis = GetReferenceTableYAxis(voieXML); // Y axis reference table

                // Second step : Get startingDate, measuringStep, precision and rawDatas
                startingDate = GetStartingDate(voieXML);
                measuringStep = GetMeasuringStep(voieXML);
                precision = GetPrecision(voieXML);
                rawDatas = GetRawDatas(voieXML);

                if (xAxis == null || yAxis == null || startingDate == null || measuringStep == 0 || rawDatas == null)
                {
                    Console.WriteLine($"\n--- Voie impossible to read");
                    return null;
                }
                else if (rawDatas.Length == 0)
                {
                    Console.WriteLine($"\n--- There is no data to read");
                    return null;
                }

                // Third step : convert each string in rawDatas into Mesure
                mesuresOfThisVoie = GetMesuresFromRawDatas(xAxis, yAxis, (DateTime)startingDate, measuringStep, precision, rawDatas, voieInterne.Numero, voieInterne.Nom);

                // Last step : filter the list with delta
                mesuresOfThisVoie = FilterWithDelta(mesuresOfThisVoie, voieInterne.Delta, voieInterne.Virgule);

            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    PrintUtils.PrintConsoleException(ex, "Unexpected format exception");
                }
                mesuresOfThisVoie = null;
            }

            return mesuresOfThisVoie;
        }

        public static List<Mesure> GetMesuresFromRawDatas(int[] xAxis, double[] yAxis, DateTime startingDate, int measuringStep, int precision, string[] rawDatas, int numeroVoie, string nomVoie)
        {
            List<Mesure> mesures = new List<Mesure>();

            for (int i = 0; i < rawDatas.Length; i++)
            {
                int rawData = Convert.ToInt32(rawDatas[i], 16); // string hexa -> int decimal
                double valeurRelative = CalculateValeurRelative(xAxis, yAxis, rawData);
                valeurRelative = Math.Round(valeurRelative, precision);
                DateTime time = startingDate;

                Mesure mesure = new Mesure {
                    
                    Time = time.AddSeconds(measuringStep * i),
                    NumeroVoie = numeroVoie,
                    NomVoie = nomVoie,
                    ValeurRelative = (float)valeurRelative
                }; 
                mesures.Add(mesure);
            }

            return mesures;
        }

        public static List<Mesure> FilterWithDelta(List<Mesure> mesures, int delta, int virgule)
        {
            double realDelta = delta * Math.Pow(10, virgule - 4);
            Mesure lastRecorededMesure = mesures[0];
            List<Mesure> recordedMesures = new List<Mesure>(); 
            recordedMesures.Add(lastRecorededMesure); // Always record the first mesure

            for (int i = 1; i < mesures.Count; i++)
            {
                Mesure mesure = mesures[i];
                if (mesure.Time.Hour == 0 && mesure.Time.Minute == 0 && mesure.Time.Second == 0)
                {
                    // Always record mesure à 00:00:00
                    recordedMesures.Add(mesure);
                    lastRecorededMesure = mesure;
                }
                else if (Math.Abs(mesure.ValeurRelative - lastRecorededMesure.ValeurRelative) >= realDelta)
                {
                    recordedMesures.Add(mesure);
                    lastRecorededMesure = mesure;
                }
            }

            return recordedMesures;
        }

        private static int[]? GetReferenceTableXAxis(XmlNode voie)
        {
            int[]? xAxis = new int[16];
            string? rawtable = XMLUtils.GetInnerText(voie, @"rawtable");

            if (rawtable != null)
            {
                string[] splitTable = rawtable.Trim().Split(" ");
                for (int i = 0; i < splitTable.Length; i++) // string hexa -> int
                {
                    string hexa = splitTable[i];
                    xAxis[i] = Convert.ToInt32(hexa, 16);
                }
            }
            else
            {
                xAxis = null;
            }

            return xAxis;
        }

        private static double[]? GetReferenceTableYAxis(XmlNode voie)
        {
            double[]? yAxis = new double[16];
            string? engtable = XMLUtils.GetInnerText(voie, @"engtable");

            if (engtable != null)
            {
                string[] splitTable = engtable.Trim().Replace(".", ",").Split(" ");
                for (int i = 0; i < splitTable.Length; i++)
                {
                    yAxis[i] = double.Parse(splitTable[i]);
                }
            }
            else
            {
                yAxis = null;
            }

            return yAxis;
        }

        private static DateTime? GetStartingDate(XmlNode voie)
        {
            DateTime? startingDate = null;
            string? startingDateStr = XMLUtils.GetInnerText(voie, @"tfrom");
            if (startingDateStr != null)
            {
                startingDate = DateTime.Parse(startingDateStr);
            }

            return startingDate;
        }

        private static int GetMeasuringStep(XmlNode voie)
        {
            int measuringStep = 0;
            string? measuringStepStr = XMLUtils.GetInnerText(voie, @"storint");

            if (measuringStepStr != null)
            {
                if (measuringStepStr.Contains('h'))
                {
                    measuringStep = int.Parse(measuringStepStr.Substring(0, measuringStepStr.Length - 1)) * 3600;
                }
                else if (measuringStepStr.Contains('m'))
                {
                    measuringStep = int.Parse(measuringStepStr.Substring(0, measuringStepStr.Length - 1)) * 60;
                }
                else
                {
                    measuringStep = int.Parse(measuringStepStr.Substring(0, measuringStepStr.Length - 1)) * 2; // Why * 2 ?
                }
            }

            return measuringStep;
        }

        private static int GetPrecision(XmlNode voie)
        {
            int precision = 0;
            string? precisionStr = XMLUtils.GetInnerText(voie, @"decimals");

            if (precisionStr != null)
            {
                precision = int.Parse(precisionStr);
            }

            return precision;
        }

        private static string[]? GetRawDatas(XmlNode voie)
        {
            string[]? rawDatasTable = null;
            string? rawDatasTableLine = XMLUtils.GetInnerText(voie, @"rawdata");

            if (rawDatasTableLine != null)
            {
                rawDatasTable = rawDatasTableLine.Trim().Split(" ");
            }

            return rawDatasTable;
        }

        private static double CalculateValeurRelative(int[] xAxis, double[] yAxis, int rawData)
        {
            int i;
            double valeurRelative;

            if (rawData < 0 || rawData >= xAxis[xAxis.Length - 1]) { return 9999; } // Value is not in reference table

            for (i = 0; i < xAxis.Length; i++) 
            {
                if (xAxis[i] >= rawData ) { break; }           
            }

            valeurRelative = (rawData - xAxis[i-1]) / (double)(xAxis[i] -xAxis[i-1]);
            valeurRelative *= (yAxis[i] - yAxis[i-1]);
            valeurRelative += yAxis[i-1];
            return valeurRelative;
        }
    }
}

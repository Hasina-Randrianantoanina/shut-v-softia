using Dapper;
using InfluxDB.Client;
using SHUT.Core.Data;
using SHUT.Core.Domain;

namespace SHUT.Core.Application.Historique
{
    public class VisualisationService
    {
        private readonly ArchiveDbContext _context;
        private readonly List<string> evenementMesure = new List<string>{
            "'L'", "'J'", "' '", "'-'", "'I'", // Ana
            "'R'","'S'","'O'","'&'", // TOR STEN
            "'r'","'s'","'o'","'c'" // TOR automate
        };
        private readonly List<string> SystemSchemaNames = new List<string> {
            "pg_catalog",
            "pg_toast",
            "public",
            "information_schema",
            "timescaledb_experimental",
            "timescaledb_information",
            "_timescaledb_internal",
            "_timescaledb_cache",
            "_timescaledb_config",
            "_timescaledb_functions",
            "_timescaledb_catalog",
            "_timescaledb_debug",
        };

        public VisualisationService(ArchiveDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetStationsAVisualiser()
        {
            try
            {
                var query = "SELECT schema_name FROM information_schema.schemata;";
                using var connection = _context.CreateConnection();
                var schemas = await connection.QueryAsync<string>(query);
                List<string> stations = schemas.Where(x => !SystemSchemaNames.Contains(x)).ToList();
                return stations;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des stations à visualiser", ex);
            }
        }

        public async Task<List<string>> GetVoiesStation(string station)
        {
            try
            {
                var query = $"SELECT table_name FROM information_schema.tables WHERE table_schema = '{station}';";

                using var connection = _context.CreateConnection();
                var voies = await connection.QueryAsync<string>(query);

                return voies.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des voies pour la station {station}", ex);
            }
        }

        public async Task<List<HistoricalDataDTO>> GetHistoricalData(string station, string voie, DateTime start, DateTime end)
        {
            try
            {
                if (start > end)
                {
                    var tmp = end;
                    end = start;
                    start = tmp;
                }

                var startTime = start.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
                string evenements = string.Join(",", evenementMesure);
                var query = $"SELECT * FROM {station}.{voie}\r\nWHERE evenement IN ({evenements}) AND horodate BETWEEN TO_TIMESTAMP('{startTime}', 'YYYY-MM-DD HH24:mi:SS')  AND TO_TIMESTAMP('{endTime}', 'YYYY-MM-DD HH24:mi:SS')\r\nORDER BY horodate";

                using var connection = _context.CreateConnection();
                var dataPoints = await connection.QueryAsync<HistoricalDataPoint>(query);
                return dataPoints.Select(x => new HistoricalDataDTO(station, voie, x.Horodate ?? "", x.Mesure ?? "")).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des données historiques pour la station {station}, voie {voie}", ex);
            }
        }
        public async Task<List<HistoricalDataDTO>> GetHistoricalDataForMultipleStationsAndVoies(string[] stations, Dictionary<string, string> stationVoiesPairs, DateTime start, DateTime end)
        {
            try
            {
                var startTime = start.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
                string evenements = string.Join(",", evenementMesure);
                var query = "";

                foreach (var station in stations)
                {
                    if (stationVoiesPairs.TryGetValue(station, out var voiesString))
                    {
                        var stationVoies = voiesString.Split(',');
                        foreach (var voie in stationVoies)
                        {
                            query += $"SELECT horodate, evenement, mesure, mesure_brute, '{voie}' AS voie, '{station}' AS station";
                            query += $" FROM {station}.{voie}";
                            query += $" WHERE evenement IN ({evenements}) AND horodate BETWEEN TO_TIMESTAMP('{startTime}', 'YYYY-MM-DD HH24:mi:SS')  AND TO_TIMESTAMP('{endTime}', 'YYYY-MM-DD HH24:mi:SS')";
                            query += " UNION ALL ";
                        }
                    }
                }

                // foreach (var station in stations)
                // {
                //     if (stationVoiesPairs.TryGetValue(station, out var voiesString))
                //     {
                //         var stationVoies = voiesString.Split(',');
                //         foreach (var voie in stationVoies)
                //         {
                //             query += $"SELECT horodate, evenement, mesure, mesure_brute, '{voie}' AS voie, '{station}' AS station";
                //             query += $" FROM {station}.{voie}";
                //             query += $" WHERE evenement IN ({evenements})";
                //             query += " UNION ALL ";
                //         }
                //     }
                // }

                if (!string.IsNullOrEmpty(query))
                {
                    query = query.Substring(0, query.Length - " UNION ALL ".Length);
                    query += " ORDER BY horodate;";

                    Console.WriteLine($"Executing query: {query}"); // Log the query

                    using var connection = _context.CreateConnection();
                    var dataPoints = await connection.QueryAsync<HistoricalDataPoint>(query);
                    return dataPoints.Select(x => new HistoricalDataDTO(x.Station ?? "", x.Voie ?? "", x.Horodate ?? "", x.Mesure ?? "")).ToList();
                }
                else
                {
                    Console.WriteLine("No query generated");
                    return new List<HistoricalDataDTO>();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetHistoricalDataForMultipleStationsAndVoies: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
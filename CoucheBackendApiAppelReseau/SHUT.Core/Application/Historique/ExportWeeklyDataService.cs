using SHUT.Core.Data;
using SHUT.Core.Domain.Reseau;
using SHUT.Core.Domain;
using Dapper;

namespace SHUT.Core.Application.Historique
{
    public class ExportWeeklyDataService
    {
        private readonly ArchiveDbContext _archiveDbContext;
        private readonly AppDbContext _appDbContext;

        private readonly List<string> evenementSHUE = new List<string>{
            "'L'", "'J'", "' '", "'-'", "'I'", // Ana + TODO : Rajouter $
        };


        public ExportWeeklyDataService(AppDbContext appContext, ArchiveDbContext archiveContext)
        {
            _appDbContext = appContext;
            _archiveDbContext = archiveContext;
        }

        public async Task<bool> GetWeeklyDatainCSVFormat(StationsReseau station, DateTime start, DateTime end)
        {
            try
            {
                //TODO : Conversion en TU
                var startTime = start.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
                string evenements = string.Join(",", evenementSHUE);
                var query = "";

                //TODO : Get voies 
                IEnumerable<VoiesTelemesurees> stationVoies;
                var queryTelemesurees = @"
                    SELECT vt.* 
                    FROM reseau.voies_telemesurees vt
                    JOIN reseau.stations s ON vt.station_id = s.id
                    WHERE s.initiales = @Initiales";
                using (var connectionDb = _appDbContext.CreateConnection())
                {
                    stationVoies = await connectionDb.QueryAsync<VoiesTelemesurees>(queryTelemesurees , new { Initiales = station.Initiales });
                }
                List<List<HistoricalDataPoint>> result = new();
                using (var connectionArchive = _archiveDbContext.CreateConnection())
                {
                    foreach (var voie in stationVoies)
                    {
                        query = $"SELECT horodate, evenement, mesure, mesure_brute, '{voie.Libelle?.ToLower()}' AS voie, '{station.Initiales.ToLower()}' AS station";
                        query += $" FROM {station.Initiales.ToLower()}.{voie.Libelle?.ToLower()}";
                        query += $" WHERE evenement IN ({evenements}) AND horodate BETWEEN TO_TIMESTAMP('{startTime}', 'YYYY-MM-DD HH24:mi:SS')  AND TO_TIMESTAMP('{endTime}', 'YYYY-MM-DD HH24:mi:SS')";
                        query += " ORDER BY horodate;";


                        var dataPoints = await connectionArchive.QueryAsync<HistoricalDataPoint>(query);
                        if (dataPoints is not null)
                        {
                            result.Add(dataPoints.AsList());
                        }
                    }
                }

                var unionResult = result.SelectMany(innerList => innerList).ToList();
                //TODO : why is horodate a nullable string, it has to be a non nullable DateTime
                List<(string?, string?)> union = unionResult.Select(x=> (x.Horodate, x.Evenement)).Distinct().ToList();
                List<HistoricalDataVoieCsv> dataVoie = new();
                foreach (var datavoie in result)
                {
                    var resultJoin = union.
                        GroupJoin(
                        datavoie,
                        timedEvent => timedEvent,
                        datapoint => (datapoint.Horodate,datapoint.Evenement),
                        (str, datapoint) => datapoint.FirstOrDefault()?.Mesure ?? "")
                        .ToList();
                    string voie = datavoie.FirstOrDefault()?.Voie ?? "";
                    dataVoie.Add(new HistoricalDataVoieCsv(voie, resultJoin));
                }
                HistoricalDataCsvFormat FinalResult = new HistoricalDataCsvFormat(union, dataVoie);
                return true;
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

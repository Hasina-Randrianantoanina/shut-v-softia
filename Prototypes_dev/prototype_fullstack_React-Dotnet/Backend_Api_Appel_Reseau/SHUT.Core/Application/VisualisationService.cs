﻿using Dapper;
using InfluxDB.Client;
using SHUT.Core.Data;
using SHUT.Core.Domain;

namespace SHUT.Core.Application
{
    public class VisualisationService
    {
        private readonly AppDbContext _context;
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
        public VisualisationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetStationsAVisualiser()
        {
            var query = "SELECT schema_name FROM information_schema.schemata;";
            using var connection = _context.CreateConnection();
            var schemas = await connection.QueryAsync<string>(query);
            List<string> stations = schemas.Where(x => !SystemSchemaNames.Contains(x)).ToList();
            return stations;
        }

        public async Task<List<string>> GetVoiesStation(string station)
        {
            var query = $"SELECT table_name FROM information_schema.tables WHERE table_schema = '{station}';";

            using var connection = _context.CreateConnection();
            var voies = await connection.QueryAsync<string>(query);
            
            return voies.ToList();
        }
        public async Task<List<HistoricalDataDTO>> GetHistoricalData(string station, string voie, DateTime start, DateTime end)
        {
            //dubious
            if (start > end)
            {
                var tmp = end;
                end = start;
                start = tmp;
            }

            var startTime = start.ToString("yyyy-MM-dd HH:mm:ss");
            var endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
            string evenements = String.Join(",",evenementMesure);
            var query = $"SELECT * FROM {station}.{voie}\r\nWHERE evenement IN ({evenements}) AND horodate BETWEEN TO_TIMESTAMP('{startTime}', 'YYYY-MM-DD HH24:mi:SS')  AND TO_TIMESTAMP('{endTime}', 'YYYY-MM-DD HH24:mi:SS')\r\nORDER BY horodate";
            
            using var connection = _context.CreateConnection();
            var dataPoints = await connection.QueryAsync<HistoricalDataPoint>(query);
            return dataPoints.Select(x => new HistoricalDataDTO(voie, x.Horodate, x.Mesure)).ToList();
        }
        public async Task<List<HistoricalDataDTO>> GetHistoricalDataForMultipleVoies(string station, List<string> voies, DateTime start, DateTime end)
        {
            var startTime = start.ToString("yyyy-MM-dd HH:mm:ss");
            var endTime = end.ToString("yyyy-MM-dd HH:mm:ss");
            string evenements = String.Join(",",evenementMesure);
            var query = "";
            for (int i = 0; i < voies.Count; i++)
            {
                query += $"SELECT horodate, evenement, mesure, mesure_brute, '{voies[i]}' AS voie";
                query += $" FROM {station}.{voies[i]}";
                query += $" WHERE evenement IN ({evenements}) AND horodate BETWEEN TO_TIMESTAMP('{startTime}', 'YYYY-MM-DD HH24:mi:SS')  AND TO_TIMESTAMP('{endTime}', 'YYYY-MM-DD HH24:mi:SS')";
                query += (i != voies.Count -1) ? " UNION " : "";
            }
            query += " ORDER BY horodate;";
            
            using var connection = _context.CreateConnection();
            var dataPoints = await connection.QueryAsync<HistoricalDataPoint>(query);
            return dataPoints.Select(x => new HistoricalDataDTO(x.Voie, x.Horodate, x.Mesure)).ToList();
        }
    }
}

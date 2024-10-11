using Dapper;
using Npgsql;
using ConnexionTR.Entities;
using ConnexionTR.Utils;

namespace ConnexionTR.Repository
{
    public class VoieInterneRepository : IVoieInterneRepository
    {
        private readonly NpgsqlConnection _connection = new NpgsqlConnection("Host=localhost;Database=shutweb_prod;Username=postgres;Password=rrrrr;");

        public async Task<List<VoieInterne>> GetAllOfAStationAsync(int stationId)
        {
            string query = $"""
                SELECT
                adresse_bes AS AdresseBES,
                delta AS ValeurDelta,
                groupe,
                info,
                numero,
                seuil_bas AS SeuilBas,
                seuil_haut AS SeuilHaut,
                voie_enregistree AS VoieEnregistree
                FROM reseau.voies_telemesurees
                WHERE station_id = '{stationId}'
                """;
            List<VoieInterne> voiesInternes = (List<VoieInterne>)await _connection.QueryAsync<VoieInterne>(query);
            
            foreach (VoieInterne voie in voiesInternes)
            {
                Int16 info = (Int16)voie.Info;
                voie.Capteur = ConversionUtils.IsThisBitValueOne(info, 1);
                voie.EnregistrementMinuit = ConversionUtils.IsThisBitValueOne(info, 2);
                voie.SensSeuil = ConversionUtils.IsThisBitValueOne(info, 3);
                voie.EnregistrementStandard = ConversionUtils.IsThisBitValueOne(info, 4);
            }
            
            return voiesInternes;
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}

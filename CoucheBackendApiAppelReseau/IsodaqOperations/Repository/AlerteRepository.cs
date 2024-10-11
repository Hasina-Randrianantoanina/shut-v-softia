using Dapper;
using IsodaqOperations.Entities;
using IsodaqOperations.Utils;
using System.Data;

namespace IsodaqOperations.Repository
{
    public class AlerteRepository
    {
        private readonly IDbConnection _connection = DataBaseUtils.AppDbContext.CreateConnection();
        const int TypeAlert = 15;
        public async Task<List<Alerte>> GetAlertes(int StationID)
        {
            string query = $"SELECT * FROM defaut.alertes WHERE station_id = {StationID} AND type = {TypeAlert}";
            var result = await _connection.QueryAsync<Alerte>(query);
            return result.AsList();
        }

        public async Task<bool> PersistAlertes(double Seuil, string Niveau, int StationId)
        {
            bool result = true;
            List<Alerte> alertesExistantes = await GetAlertes(StationId);
            string query = string.Empty ;
            if(!double.TryParse(Niveau, out double niveau)) { return false; }
            if (niveau < Seuil)
            {
                Alerte alerte = new Alerte()
                {
                    Acquitter = false,
                    Commentaire = "",
                    DateAlerte = DateTime.UtcNow,
                    DescriptionAlerte = $"Tension batterie : {niveau.ToString("0.00")} < {Seuil.ToString("0.00")}",
                    ParametreAction ="",
                    ParametreAlerte = DateTime.Now.ToShortDateString(),
                    StationId = StationId,
                    Type = TypeAlert,
                };
                if (alertesExistantes?.Count == 0)
                {
                    query = "INSERT INTO defaut.alertes (acquitter, commentaire, date_alerte, description_alerte, parametre_action, parametre_alerte, station_id, type) VALUES (@Acquitter, @Commentaire, @DateAlerte, @DescriptionAlerte, @ParametreAction, @ParametreAlerte, @StationId, @Type)";
                    result = await _connection.ExecuteAsync(query, alerte) > 0;
                }
                else
                {
                    query = $"UPDATE defaut.alertes SET parametreAlerte = {alerte.ParametreAlerte} , description_alerte = {alerte.DescriptionAlerte} WHERE station_id = {alerte.StationId} AND Type = {alerte.Type}";
                    result = await _connection.ExecuteAsync(query) > 0;
                }
            }
            else
            {
                if (alertesExistantes?.Count > 0)
                {
                    query = $"DELETE FROM defaut.alertes WHERE station_id = {StationId} AND type = {TypeAlert}";
                    result = await _connection.ExecuteAsync(query) > 0;
                }
            }
            return result;
        }
    }
}

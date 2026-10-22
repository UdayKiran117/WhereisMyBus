using Dapper;
using MySqlConnector;
using WhereisMyBus.DTO;

namespace WhereIsMyBus.API.Model.AgencyBL
{
    public class AgencyGetBL : IDisposable
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;
        private readonly ILogger<AgencyGetBL> _logger;
        public AgencyGetBL(IConfiguration configuration, ILogger<AgencyGetBL> logger)
        {
            _logger = logger;
            _connectionString = configuration["Database:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Database connection string is missing.");

            _commandTimeout = configuration.GetValue<int>(
                "Database:CommandTimeoutSeconds", 15);
        }
       

public async Task<List<AgentDTO>> GetAllAgentsAsync(string? agencyName = null)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);

                const string sql = @"
    SELECT
        a.agent_id AS AgentId,
        a.agency_name AS AgencyName,
        a.mobile AS Mobile,
        a.alternate_mobile AS AlternateMobile,

        a.state_id AS StateId,

        a.district_id AS DistrictId,
        d.district_name AS DistrictName,
        d.district_short_name AS DistrictShortName,

        a.mandal_id AS MandalId,
        m.mandal_name AS MandalName,
        m.mandal_short_name AS MandalShortName,

        a.village_id AS VillageId,
        v.village_name AS VillageName,
        v.village_short_name AS VillageShortName,

        a.GSTIN AS GSTIN,
        a.Latitude AS Latitude,
        a.Longitude AS Longitude

    FROM agents a

    LEFT JOIN districts d
        ON a.district_id = d.district_id

    LEFT JOIN mandals m
        ON a.mandal_id = m.mandal_id
        AND m.district_id = d.district_id

    LEFT JOIN villages v
        ON a.village_id = v.village_id
        AND v.mandal_fk = m.mandal_id

    WHERE
        (@AgencyName IS NULL
         OR a.agency_name LIKE @SearchPattern)

    ORDER BY a.agency_name ASC";

                var parameters = new
            {
                AgencyName = string.IsNullOrWhiteSpace(agencyName)
                    ? null
                    : agencyName,

                SearchPattern = $"%{agencyName?.Trim()}%"
            };

            var agents = await connection.QueryAsync<AgentDTO>(
                sql,
                parameters,
                commandTimeout: _commandTimeout);

            return agents.ToList();
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex,
                    "MySQL error while fetching agents. AgencyName: {AgencyName}",
                    agencyName);

                return new List<AgentDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while fetching agents. AgencyName: {AgencyName}",
                    agencyName);

                return new List<AgentDTO>();
            }
        }
       public void Dispose()
        {
            
        }
    }
}

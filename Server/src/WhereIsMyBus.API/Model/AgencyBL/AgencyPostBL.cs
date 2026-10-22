using Dapper;
using MySqlConnector;
using WhereisMyBus.DTO;
using WhereIsMyBus.API.Model.AgencyBL;

namespace WhereIsMyBus.API.Model.BL
{
    public class AgencyPostBL : IDisposable
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;
        private readonly ILogger<AgencyGetBL> _logger;
        public void Dispose()
        {
        }
        public AgencyPostBL(IConfiguration configuration, ILogger<AgencyGetBL> logger)
        {
            _logger = logger;

            _connectionString = configuration["Database:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Database connection string is missing.");

            _commandTimeout = configuration.GetValue<int>(
                "Database:CommandTimeoutSeconds", 15);
        }

        public async Task<InsertAgentResponseDTO> InsertAgentAsync(AgentDTO agent)
        {
            try
            {
                if (agent == null)
                {
                    return new InsertAgentResponseDTO
                    {
                        Status = false,
                        Message = "Agent details are required."
                    };
                }

                if (string.IsNullOrWhiteSpace(agent.AgencyName))
                {
                    return new InsertAgentResponseDTO
                    {
                        Status = false,
                        Message = "Agency name is required."
                    };
                }

                if (string.IsNullOrWhiteSpace(agent.Mobile))
                {
                    return new InsertAgentResponseDTO
                    {
                        Status = false,
                        Message = "Mobile number is required."
                    };
                }

                if (agent.AgencyName.Length > 100 ||
                    agent.Mobile.Length > 13 ||
                    agent.AlternateMobile?.Length > 13 ||
                    agent.GSTIN?.Length > 15 ||
                    agent.Latitude?.Length > 50 ||
                    agent.Longitude?.Length > 50 ||
                    agent.VillageId?.Length > 4)
                {
                    return new InsertAgentResponseDTO
                    {
                        Status = false,
                        Message = "One or more field values exceed the allowed length."
                    };
                }

                await using var connection =
                    new MySqlConnection(_connectionString);

                await connection.OpenAsync();

                const string sql = @"
            INSERT INTO agents
            (
                agency_name,
                mobile,
                alternate_mobile,
                state_id,
                district_id,
                mandal_id,
                village_id,
                GSTIN,
                Latitude,
                Longitude
            )
            VALUES
            (
                @AgencyName,
                @Mobile,
                @AlternateMobile,
                @StateId,
                @DistrictId,
                @MandalId,
                @VillageId,
                @GSTIN,
                @Latitude,
                @Longitude
            );

            SELECT LAST_INSERT_ID();";

                var agentId = await connection.ExecuteScalarAsync<long>(
                    sql,
                    new
                    {
                        agent.AgencyName,
                        agent.Mobile,
                        agent.AlternateMobile,
                        agent.StateId,
                        agent.DistrictId,
                        agent.MandalId,
                        agent.VillageId,
                        agent.GSTIN,
                        agent.Latitude,
                        agent.Longitude
                    },
                    commandTimeout: _commandTimeout);

                _logger.LogInformation(
                    "Agent inserted successfully. AgentId: {AgentId}",
                    agentId);

                return new InsertAgentResponseDTO
                {
                    Status = true,
                    Message = "Agent inserted successfully.",
                    AgentId = agentId
                };
            }
            catch (MySqlException ex)
            {
                _logger.LogError(
                    ex,
                    "MySQL error while inserting agent: {AgencyName}",
                    agent?.AgencyName);

                return new InsertAgentResponseDTO
                {
                    Status = false,
                    Message = "Database error occurred while saving the agent."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while inserting agent: {AgencyName}",
                    agent?.AgencyName);

                return new InsertAgentResponseDTO
                {
                    Status = false,
                    Message = "Unable to insert agent."
                };
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WhereisMyBus.DTO;
using WhereIsMyBus.API.Model.AgencyBL;
using WhereIsMyBus.API.Model.BL;

namespace WhereIsMyBus.API.Controllers
{
    
        [ApiController]
        [Route("api/[controller]")]
        public class AgentsController : ControllerBase
        {
            private readonly AgencyGetBL _agentBL;
            private readonly AgencyPostBL _agentpostBL;
            private readonly ILogger<AgentsController> _logger;

            public AgentsController(AgencyPostBL agentpostBL,ILogger<AgentsController> logger)
            {
                _agentpostBL = agentpostBL;
                _logger = logger;
            }

            [HttpGet("GetAllAgents")]
            public async Task<IActionResult> GetAllAgents([FromQuery] string? agencyName = null)
            {
                try
                {
                    var agents = await _agentBL.GetAllAgentsAsync(agencyName);

                    if (agents == null || agents.Count == 0)
                    {
                        return NotFound(new
                        {
                            Status = false,
                            Message = "No agents found."
                        });
                    }

                    return Ok(new
                    {
                        Status = true,
                        Message = "Agents fetched successfully.",
                        TotalCount = agents.Count,
                        Data = agents
                    });
                }
                catch (MySqlException ex)
                {
                    _logger.LogError(ex,
                        "Database error while fetching agents.");

                    return StatusCode(500, new
                    {
                        Status = false,
                        Message = "Database error occurred."
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error while fetching agents.");

                    return StatusCode(500, new
                    {
                        Status = false,
                        Message = "An unexpected error occurred."
                    });
                }
            }

        [HttpPost("InsertAgent")]
        public async Task<IActionResult> InsertAgent([FromBody] AgentDTO agent)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _agentpostBL.InsertAgentAsync(agent);

                if (!result.Status)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while inserting agent.");

                return StatusCode(500, new
                {
                    Status = false,
                    Message = "Internal server error."
                });
            }
        }

    }
    }
    
    


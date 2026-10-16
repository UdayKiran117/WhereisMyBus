using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Domain.Entities;

namespace WhereIsMyBus.API.Controllers
{
    
    [Route("api/drivers")]
    public sealed class DriversController(IDriverRepository repository) : ControllerBase
    {
        [HttpPost("registerdriver")]
        public async Task<ActionResult<DriverInformation>> RegisterDriver([FromBody] DriverDetails request, CancellationToken ct)
        {
            try
            {
                var id = await repository.SaveDriverDetailsAsync(request, ct);
                return StatusCode(StatusCodes.Status201Created, new DriverInformation(id, true, "Driver details saved successfully."));
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return Conflict(new DriverInformation(0, false, "A driver with these unique details already exists."));
            }
        }
        [HttpPost("getdriverinfo")]
        public async Task<ActionResult<DriverDetails>> GetDriverInfo([FromBody] DriverDetails request, CancellationToken ct = default)
        {
            try
            {
                var response = await repository.GetDriverDetailsAsync(request, ct);
                return Ok(response);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return Conflict(new DriverInformation(0, false, "A driver with these unique details already exists."));
            }
        }
        [HttpGet("UpdateDrivePasswordByMobile")]
        public async Task<ActionResult> UpdateDrivePasswordByMobile(string driver_mobile, string password, CancellationToken ct = default)
        {
            try
            {
                var response = await repository.UpdateDriverPassword(driver_mobile, password, ct);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return new JsonResult(null);
            }
        }
        [HttpGet("busregistrations")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetDriverBusRegistrations([FromQuery] string driverMobile, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(driverMobile))
                return BadRequest("Driver mobile is required.");

            var registrations = await repository.GetDriverBusRegistrationsAsync(driverMobile.Trim(), ct);
            return Ok(registrations);
        }
        [HttpGet("drivervillages")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetDriverVillages([FromQuery] string driverMobile, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(driverMobile))
                return BadRequest("Driver mobile is required.");

            var villages = await repository.GetDriverVillagesAsync(driverMobile.Trim(), ct);

            return Ok(villages);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using WhereIsMyBus.Domain.Entities;

namespace WhereIsMyBus.Application.Interfaces
{
    public interface IDriverRepository
    {
        Task<long> SaveDriverDetailsAsync(DriverDetails request, CancellationToken ct = default);
        Task<DriverDetails> GetDriverDetailsAsync(DriverDetails request, CancellationToken ct = default);
        Task<bool> UpdateDriverPassword(string driver_mobile, string password, CancellationToken ct = default);
        Task<IReadOnlyList<string>> GetDriverBusRegistrationsAsync(string driverMobile, CancellationToken ct = default);
        Task<IReadOnlyList<string>> GetDriverVillagesAsync(string driverMobile, CancellationToken ct = default);
    }
}

using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using WhereIsMyBus.Application.Interfaces;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Infrastructure.Configuration;
using WhereIsMyBus.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace WhereIsMyBus.Infrastructure.Repositories
{
    public sealed class DriverRepository(MySqlConnectionFactory factory, IOptions<DatabaseOptions> options, IPasswordHasher<DriverDetails> passwordHasher) : IDriverRepository
    {
        public async Task<long> SaveDriverDetailsAsync(DriverDetails request, CancellationToken ct = default)
        {
            const string sql = """
            INSERT INTO driver_logindetails
            (
                driver_name,
                driver_mobile,
                driver_alternate_mobile,
                driver_licence_no,
                `password`,
                second_driver_name,
                second_driver_mobile,
                second_driver_alternate_mobile,
                mobile_device_name,
                second_driver_licence_no,
                login_datetime,
                location_enable
            )
            VALUES
            (
                @DriverName,
                @DriverMobile,
                @DriverAlternateMobile,
                @DriverLicenceNo,
                @PasswordHash,
                @SecondDriverName,
                @SecondDriverMobile,
                @SecondDriverAlternateMobile,
                @MobileDeviceName,
                @SecondDriverLicenceNo,
                NULL,
                0
            );

            SELECT LAST_INSERT_ID();
            """;

            // Hash before opening the database connection.
            var hash = passwordHasher.HashPassword(
                request,
                request.Password);

            var parameters = new
            {
                DriverName = request.DriverName.Trim(),
                DriverMobile = request.DriverMobile,
                DriverAlternateMobile = (request.DriverAlternateMobile),
                DriverLicenceNo = request.DriverLicenceNo.Trim().ToUpperInvariant(),

                PasswordHash = Encoding.UTF8.GetBytes(hash),

                SecondDriverName = request.SecondDriverName.Trim(),
                SecondDriverMobile = (request.SecondDriverMobile),
                SecondDriverAlternateMobile = (request.SecondDriverAlternateMobile),
                SecondDriverLicenceNo = request.SecondDriverLicenceNo.Trim().ToUpperInvariant(),

                MobileDeviceName = string.IsNullOrWhiteSpace(request.MobileDeviceName) ? null : request.MobileDeviceName.Trim()
            };

            await using var db = await factory.OpenAsync(ct);

            return await db.ExecuteScalarAsync<long>(
                new CommandDefinition(
                    sql,
                    parameters,
                    commandTimeout: options.Value.CommandTimeoutSeconds,
                    cancellationToken: ct));
        } 
        public async Task<DriverDetails> GetDriverDetailsAsync(DriverDetails details, CancellationToken ct = default)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(details);

                if (string.IsNullOrWhiteSpace(details.Password))
                    throw new ArgumentException("Password is required."); 

                var parameters = new
                {
                    DriverMobile = (details.DriverMobile),
                    DriverAlternateMobile =
                        (details.DriverAlternateMobile),
                    SecondDriverMobile =
                        (details.SecondDriverMobile),
                    SecondDriverAlternateMobile =
                        (details.SecondDriverAlternateMobile)
                };

                if (parameters.DriverMobile is null &&
                    parameters.DriverAlternateMobile is null &&
                    parameters.SecondDriverMobile is null &&
                    parameters.SecondDriverAlternateMobile is null)
                {
                    throw new ArgumentException("Enter at least one mobile number.");
                }

                const string sql = """
            SELECT
                driver_name AS DriverName,
                driver_mobile AS DriverMobile,
                driver_alternate_mobile AS DriverAlternateMobile,
                driver_licence_no AS DriverLicenceNo,
                second_driver_name AS SecondDriverName,
                second_driver_mobile AS SecondDriverMobile,
                second_driver_alternate_mobile AS SecondDriverAlternateMobile,
                mobile_device_name AS MobileDeviceName,
                second_driver_licence_no AS SecondDriverLicenceNo,
                login_datetime AS LoginDatetime,
                location_enable AS LocationEnable,
                `password` AS PasswordHash
            FROM driver_logindetails
            WHERE
            (
                driver_mobile = @DriverMobile
                OR driver_alternate_mobile = @DriverAlternateMobile
                OR second_driver_mobile = @SecondDriverMobile
                OR second_driver_alternate_mobile =
                    @SecondDriverAlternateMobile
            )
            AND location_enable = 0;
            """;

                await using var db = await factory.OpenAsync(ct);

                var rows = (await db.QueryAsync<DriverDetailsRow>(
                    new CommandDefinition(
                        sql,
                        parameters,
                        commandTimeout: options.Value.CommandTimeoutSeconds,
                        cancellationToken: ct))).AsList();

                // Do not silently choose an account when mobile numbers match
                // multiple records.
                if (rows.Count != 1)
                    throw new UnauthorizedAccessException(
                        "Invalid credentials or account unavailable.");

                var row = rows[0];

                if (row.PasswordHash is null || row.PasswordHash.Length == 0)
                    throw new UnauthorizedAccessException(
                        "Invalid credentials or account unavailable.");

                var storedHash = Encoding.UTF8.GetString(row.PasswordHash);

                // Uses your existing IPasswordHasher<SaveDriverRequest>.
                var verification = passwordHasher.VerifyHashedPassword(new DriverDetails(), storedHash, details.Password);

                if (verification == PasswordVerificationResult.Failed)
                    throw new UnauthorizedAccessException(
                        "Invalid credentials or account unavailable.");

                return new DriverDetails
                {
                    DriverName = row.DriverName,
                    DriverMobile = row.DriverMobile,
                    DriverAlternateMobile = row.DriverAlternateMobile,
                    DriverLicenceNo = row.DriverLicenceNo,
                    SecondDriverName = row.SecondDriverName,
                    SecondDriverMobile = row.SecondDriverMobile,
                    SecondDriverAlternateMobile = row.SecondDriverAlternateMobile,
                    MobileDeviceName = row.MobileDeviceName,
                    SecondDriverLicenceNo = row.SecondDriverLicenceNo,
                    LoginDatetime = row.LoginDatetime,
                    LocationEnable = row.LocationEnable

                    // Do not return the password or stored hash.
                };
            }
            catch (Exception)
            {
                // Preserve the original exception for your global handler.
                // This catch can be removed if no additional handling is needed.
                throw;
            }
        }

        public async Task<bool> UpdateDriverPassword(string driver_mobile, string password, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(driver_mobile))
                throw new ArgumentException("Driver mobile is required.", nameof(driver_mobile));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            var request = new DriverDetails
            {
                DriverMobile = driver_mobile.Trim(),
                Password = password
            };

            var hashedPassword = passwordHasher.HashPassword(request, password);

            // Existing password column is BLOB.
            var passwordBytes = Encoding.UTF8.GetBytes(hashedPassword);

            const string sql = """
        UPDATE driver_logindetails
        SET password = @Password
        WHERE driver_mobile = @DriverMobile;
        """;

            await using var db = await factory.OpenAsync(ct);

            var affectedRows = await db.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new
                    {
                        Password = passwordBytes,
                        DriverMobile = driver_mobile.Trim()
                    },
                    commandTimeout: options.Value.CommandTimeoutSeconds,
                    cancellationToken: ct));

            return affectedRows > 0;
        }
        public async Task<IReadOnlyList<string>> GetDriverBusRegistrationsAsync(string driverMobile, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(driverMobile))
                throw new ArgumentException("Driver mobile is required.", nameof(driverMobile));

            const string sql = """
        SELECT DISTINCT c.registration_no
        FROM driver_logindetails a
        JOIN agents b ON a.agent_id_fk = b.agent_id
        JOIN buses c ON c.driver_fk = a.driver_id
        WHERE (
            a.driver_mobile = @DriverMobile
            OR a.driver_alternate_mobile = @DriverMobile
            OR a.second_driver_mobile = @DriverMobile
            OR a.second_driver_alternate_mobile = @DriverMobile
        )
        ORDER BY c.registration_no;
        """;

            await using var db = await factory.OpenAsync(ct);

            var registrations = await db.QueryAsync<string>(
                new CommandDefinition(
                    sql,
                    new { DriverMobile = driverMobile.Trim() },
                    commandTimeout: options.Value.CommandTimeoutSeconds,
                    cancellationToken: ct));

            return registrations.AsList();
        }
        public async Task<IReadOnlyList<string>> GetDriverVillagesAsync(string driverMobile, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(driverMobile))
                throw new ArgumentException(
                    "Driver mobile is required.", nameof(driverMobile));

            const string sql = """
        SELECT DISTINCT d.village_name
        FROM driver_logindetails a
        JOIN agents b ON a.agent_id_fk = b.agent_id
        JOIN buses c ON c.driver_fk = a.driver_id
        JOIN villages d ON d.mandal_fk = b.mandal_id
        WHERE (
            a.driver_mobile = @DriverMobile
            OR a.driver_alternate_mobile = @DriverMobile
            OR a.second_driver_mobile = @DriverMobile
            OR a.second_driver_alternate_mobile = @DriverMobile
        )
        ORDER BY d.village_name;
        """;

            await using var db = await factory.OpenAsync(ct);

            var villages = await db.QueryAsync<string>(
                new CommandDefinition(
                    sql,
                    new { DriverMobile = driverMobile.Trim() },
                    commandTimeout: options.Value.CommandTimeoutSeconds,
                    cancellationToken: ct));

            return villages.AsList();
        }
    }
}

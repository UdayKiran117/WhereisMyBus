using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WhereIsMyBus.Domain.Entities
{

    public sealed class DriverDetails
    {
        [Required, StringLength(50)]
        public string DriverName { get; set; } = "";

        // Accepts 10 digits or +91 followed by 10 digits.
        [Required, RegularExpression(@"^(?:\+91)?[6-9]\d{9}$")]
        public string DriverMobile { get; set; } = "";

        [RegularExpression(@"^(?:\+91)?[6-9]\d{9}$")]
        public string? DriverAlternateMobile { get; set; }

        [Required, StringLength(100)]
        public string DriverLicenceNo { get; set; } = "";

        [Required, StringLength(128, MinimumLength = 12)]
        public string Password { get; set; } = "";

        [Required, StringLength(50)]
        public string SecondDriverName { get; set; } = "";

        [Required, RegularExpression(@"^(?:\+91)?[6-9]\d{9}$")]
        public string SecondDriverMobile { get; set; } = "";

        [RegularExpression(@"^(?:\+91)?[6-9]\d{9}$")]
        public string? SecondDriverAlternateMobile { get; set; }

        [Required, StringLength(100)]
        public string SecondDriverLicenceNo { get; set; } = "";

        [StringLength(100)]
        public string? MobileDeviceName { get; set; }

        public DateTime? LoginDatetime { get; set; }
        public bool? LocationEnable { get; set; }
    }
    public sealed class DriverDetailsRow
    {
        public string DriverName { get; set; } = "";
        public string DriverMobile { get; set; } = "";
        public string? DriverAlternateMobile { get; set; }
        public string DriverLicenceNo { get; set; } = "";

        public string? SecondDriverName { get; set; }
        public string? SecondDriverMobile { get; set; }
        public string? SecondDriverAlternateMobile { get; set; }
        public string? SecondDriverLicenceNo { get; set; }

        public string? MobileDeviceName { get; set; }
        public DateTime? LoginDatetime { get; set; }
        public bool? LocationEnable { get; set; }
        public byte[]? PasswordHash { get; set; }
    }
    public sealed record DriverInformation(
        long Id,
        bool Status,
        string Message);
}

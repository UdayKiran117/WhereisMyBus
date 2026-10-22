using System;
using System.Collections.Generic;
using System.Text;

namespace WhereisMyBus.DTO
{
    public class AgentDTO
    {
        public int AgentId { get; set; }
        public string? AgencyName { get; set; }
        public string? Mobile { get; set; }
        public string? AlternateMobile { get; set; }

        public int? StateId { get; set; }

        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? DistrictShortName { get; set; }

        public int? MandalId { get; set; }
        public string? MandalName { get; set; }
        public string? MandalShortName { get; set; }

        public string? VillageId { get; set; }
        public string? VillageName { get; set; }
        public string? VillageShortName { get; set; }

        public string? GSTIN { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}

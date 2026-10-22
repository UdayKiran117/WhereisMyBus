using System;
using System.Collections.Generic;
using System.Text;

namespace WhereisMyBus.DTO
{
    public class InsertAgentResponseDTO
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public long? AgentId { get; set; }
    }
}

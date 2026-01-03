namespace SmartTicketApi.Models.DTOs.Manager
{
    public class AgentWorkloadDto
    {
        public int AgentId { get; set; }
        public string Name { get; set; }
        public int AssignedTicketCount { get; set; }
    }

}

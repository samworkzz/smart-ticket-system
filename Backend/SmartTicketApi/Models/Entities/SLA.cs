namespace SmartTicketApi.Models.Entities
{
    public class SLA
    {
        public int SLAId { get; set; }

        public int ResponseHours { get; set; }

        //FK
        public int TicketPriorityId { get; set; }
        public TicketPriority TicketPriority { get; set; }

       
    }
}

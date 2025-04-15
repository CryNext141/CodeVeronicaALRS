namespace ALRS.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public string ActionName { get; set; }
        public string RequestData { get; set; }
        public int ResponseStatusCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

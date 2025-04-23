namespace ALRS.Models
{
    public class AlertStatus
    {
        public int AlertStatusId { get; set; }   
        public string Code { get; set; }          
        public string DisplayName { get; set; }  

        public ICollection<Alert> Alerts { get; set; }
    }
}

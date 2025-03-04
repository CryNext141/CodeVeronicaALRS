namespace ALRS.Models
{
    public class Alert
    {
        public int AlertId { get; set; }
        public int AlertStatus { get; set; }
        public bool BroadcastCancelled { get; set; } = false;
        public string CrimeLocation { get; set; }
        public string CrimeDate { get; set; }
        public Victim Victim { get; set; }
        public Abductor Abductor { get; set; }
        public ICollection<CitizenReport> CitizenReports { get; set; }


        public int? VictimId { get; set; }
        public int? AbductorId { get; set; }
    }
}

namespace ALRS.Models
{
    public class Alerts
    {
        public int Id { get; set; }
        public string? VictimName { get; set; }
        public int VictimAge { get; set; }
        public string? CrimeLocation { get; set; }
        public string? CrimeDate { get; set; }
        public int CrimeStatus { get; set; }
    }
}

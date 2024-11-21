namespace ALRS.Models
{
    public class CitizenReport
    {
        public int CitizenReportId { get; set; }
        public string CitizenName { get; set; }
        public int CitizenContactPhone { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public bool IsAnonymous { get; set; }

        public int AlertId { get; set; }
        public Alert Alert { get; set; }
    }
}

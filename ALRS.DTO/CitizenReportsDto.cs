namespace ALRS.DTO
{
    public class CitizenReportsDto
    {
        public int CitizenReportId { get; set; }
        public string CitizenName { get; set; }
        public int CitizenContactPhone { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public bool IsAnonymous { get; set; }
    }
}

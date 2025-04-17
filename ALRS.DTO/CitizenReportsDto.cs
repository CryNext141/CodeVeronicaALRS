namespace ALRS.DTO
{
    public class CitizenReportsDto
    {
        public string CitizenName { get; set; }
        public string CitizenContactPhone { get; set; }
        public string Location { get; set; }
        public ReportDateDto ReportDate { get; set; }
        public string Description { get; set; }
        public bool IsAnonymous { get; set; }
    }
}

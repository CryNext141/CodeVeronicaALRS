namespace ALRS.DTO
{
    public class GetAlertById
    {
        public int AlertStatus { get; set; }
        public string CrimeLocation { get; set; }
        public string CrimeDate { get; set; }
        public GetAlertByIdVictimDto Victim { get; set; }
        public GetAlertByIdAbductorDto Abductor { get; set; }
    }

    public class GetAlertByIdVictimDto
    {
        public string VictimName { get; set; }
        public int VictimAge { get; set; }
        public string VictimSex { get; set; }
        public string VictimHair { get; set; }
        public string VictimClothing { get; set; }
        public string VictimPhoto { get; set; }
    }

    public class GetAlertByIdAbductorDto
    {
        public string AbductorName { get; set; }
        public int AbductorAge { get; set; }
        public string AbductorSex { get; set; }
        public string AbductorHair { get; set; }
        public string AbductorClothing { get; set; }
        public string AbductorVehicle { get; set; }
        public string AbductorPhoto { get; set; }
    }
}

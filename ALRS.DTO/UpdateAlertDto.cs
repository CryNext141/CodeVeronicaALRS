namespace ALRS.DTO
{
    public class UpdateAlertDto
    {
        public int AlertStatus { get; set; }
        public string CrimeDistrict { get; set; }
        public string CrimeLocation { get; set; }
        public CrimeDateDto CrimeDate { get; set; }
        public VictimDto Victim { get; set; }
        public AbductorDto Abductor { get; set; }
    }

    public class VictimDto
    {
        public string VictimName { get; set; }
        public int VictimAge { get; set; }
        public string VictimSex { get; set; }
        public string VictimSkinColor { get; set; }
        public string VictimHair { get; set; }
        public string VictimClothing { get; set; }
        public string VictimDistinctiveFeatures { get; set; }
        public string VictimPhoto { get; set; }
    }

    public class AbductorDto
    {
        public string AbductorName { get; set; }
        public int AbductorAge { get; set; }
        public string AbductorSex { get; set; }
        public string AbductorSkinColor { get; set; }
        public string AbductorHair { get; set; }
        public string AbductorClothing { get; set; }
        public string AbductorDistinctiveFeatures { get; set; }
        public string AbductorVehicle { get; set; }
        public string AbductorPhoto { get; set; }
    }
}

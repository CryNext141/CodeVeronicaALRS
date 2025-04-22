namespace ALRS.DTO
{
    public class AlertArchiveDto
    {
        public int AlertId { get; set; }
        public int AlertStatus { get; set; }
        public string CrimeDistrict { get; set; }
        public string CrimeLocation { get; set; }
        public CrimeDateDto CrimeDate { get; set; }

        public VictimArchiveDto Victim { get; set; }
        public AbductorArchiveDto Abductor { get; set; }
    }

    public class VictimArchiveDto
    {
        public string VictimName { get; set; }
        public int? VictimAge { get; set; }
        public string VictimGender { get; set; }
        public string VictimSkinColor { get; set; }
        public string VictimHair { get; set; }
        public string VictimClothing { get; set; }
        public string VictimDistinctiveFeatures { get; set; }
        public string VictimPhoto { get; set; }
    }

    public class AbductorArchiveDto
    {
        public string AbductorName { get; set; }
        public int? AbductorAge { get; set; }
        public string AbductorGender { get; set; }
        public string AbductorSkinColor { get; set; }
        public string AbductorHair { get; set; }
        public string AbductorClothing { get; set; }
        public string AbductorDistinctiveFeatures { get; set; }
        public string AbductorVehicle { get; set; }
        public string AbductorPhoto { get; set; }
    }
}

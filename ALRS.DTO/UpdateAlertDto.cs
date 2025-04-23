namespace ALRS.DTO
{
    public class UpdateAlertDto
    {
        public int AlertStatusId { get; set; }
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
        public int GenderId { get; set; }
        public int SkinColorId { get; set; }
        public string VictimHair { get; set; }
        public string VictimClothing { get; set; }
        public string VictimDistinctiveFeatures { get; set; }
        public string VictimPhoto { get; set; }
    }

    public class AbductorDto
    {
        public string AbductorName { get; set; }
        public int AbductorAge { get; set; }
        public int GenderId { get; set; }
        public int SkinColorId { get; set; }
        public string AbductorHair { get; set; }
        public string AbductorClothing { get; set; }
        public string AbductorDistinctiveFeatures { get; set; }
        public string AbductorVehicle { get; set; }
        public string AbductorPhoto { get; set; }
    }
}

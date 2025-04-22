using System.ComponentModel.DataAnnotations;

namespace ALRS.Models
{
    public class AlertArchive
    {
        [Key]
        public int AlertId { get; set; }
        public int? AlertStatusId { get; set; }
        public string CrimeDistrict { get; set; }
        public string CrimeLocation { get; set; }
        public DateTime CrimeDate { get; set; }
        public TimeSpan CrimeTime { get; set; }

        public string VictimName { get; set; }
        public int? VictimAge { get; set; }
        public int? VictimGenderId { get; set; }
        public int? VictimSkinColorId { get; set; }
        public string VictimHair { get; set; }
        public string VictimClothing { get; set; }
        public string VictimDistinctiveFeatures { get; set; }
        public byte[] VictimPhoto { get; set; }

        public string AbductorName { get; set; }
        public int? AbductorAge { get; set; }
        public int? AbductorGenderId { get; set; }
        public int? AbductorSkinColorId { get; set; }
        public string AbductorHair { get; set; }
        public string AbductorClothing { get; set; }
        public string AbductorDistinctiveFeatures { get; set; }
        public string AbductorVehicle { get; set; }
        public byte[] AbductorPhoto { get; set; }
    }
}

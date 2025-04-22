namespace ALRS.Models
{
    public class Abductor
    {
        public int AbductorId { get; set; }
        public string AbductorName { get; set; }
        public int AbductorAge { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
        public int SkinColorId { get; set; }
        public SkinColor SkinColor { get; set; }
        public string AbductorHair { get; set;}
        public string AbductorClothing { get; set;}
        public string AbductorDistinctiveFeatures { get; set; }
        public string AbductorVehicle {  get; set;}
        public byte[] AbductorPhoto { get; set; }
        public int AlertId { get; set; }
        public Alert Alert { get; set;}
    }
}

namespace ALRS.Models
{
    public class Abductor
    {
        public int AbductorId { get; set; }
        public string AbductorName { get; set; }
        public int AbductorAge { get; set; }
        public string AbductorSex { get; set;}
        public string AbductorHair { get; set;}
        public string AbductorClothing { get; set;}
        public string AbductorVehicle {  get; set;}
        public int AlertId { get; set; }
        public Alert Alert { get; set;}
    }
}

namespace ALRS.Models
{
    public class Victim
    {
        public int VictimId { get; set; }
        public string VictimName { get; set; }
        public int VictimAge { get; set; }
        public string VictimSex { get; set; }
        public string VictimHair {  get; set; }
        public string VictimClothing {  get; set; }
        public int AlertId { get; set; }
        public Alert Alert { get; set; }
    }
}

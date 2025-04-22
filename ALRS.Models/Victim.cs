namespace ALRS.Models
{
    public class Victim
    {
        public int VictimId { get; set; }
        public string VictimName { get; set; }
        public int VictimAge { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
        public int SkinColorId { get; set; }
        public SkinColor SkinColor { get; set; }
        public string VictimHair {  get; set; }
        public string VictimClothing {  get; set; }
        public string VictimDistinctiveFeatures { get; set; }
        public byte[] VictimPhoto { get; set; }

        public int AlertId { get; set; }
        public Alert Alert { get; set; }
    }
}

namespace ALRS.Models
{
    public class Gender
    {
        public int GenderId { get; set; }
        public string Code { get; set; }        
        public string DisplayName { get; set; } 

        public ICollection<Victim> Victims { get; set; }
        public ICollection<Abductor> Abductors { get; set; }
    }

    public class SkinColor
    {
        public int SkinColorId { get; set; }
        public string Name { get; set; }        

        public ICollection<Victim> Victims { get; set; }
        public ICollection<Abductor> Abductors { get; set; }
    }
}

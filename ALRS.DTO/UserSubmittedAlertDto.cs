namespace ALRS.DTO
{
    public class UserAlertDto
    {
        public string CrimeLocation { get; set; }
        public string CrimeDate { get; set; }
        public string VictimLook { get; set; }
    }

    public class KidnapperDto
    {
        public string KidnapperName { get; set; }
        public int KidnapperAge { get; set; }
        public string KidnapperSex { get; set; }
        public string KidnapperLook { get; set; }
        public string KidnapperVehicle { get; set; }
    }
}
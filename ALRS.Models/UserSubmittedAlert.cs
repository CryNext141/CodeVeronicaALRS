namespace ALRS.Models
{
    public class UserSubmittedAlert
    {
        public int Id { get; set; }
        public string CrimeLocation { get; set; }
        public string CrimeDate { get; set; }
        public string VictimLook { get; set; }
        public KidnapperDetails? KidnapperDetails { get; set; }

        public int AlertsId { get; set; }
        public Alerts Alerts { get; set; }
    }

    public class KidnapperDetails
    {
        public int Id { get; set; }
        public string KidnapperName { get; set; }
        public int KidnapperAge { get; set; }
        public string KidnapperSex { get; set; }
        public string KidnapperLook { get; set; }
        public string KidnapperVehicle { get; set; }

        public int UserSubmittedAlertId { get; set; }
        public UserSubmittedAlert UserSubmittedAlert { get; set; }
    }
}

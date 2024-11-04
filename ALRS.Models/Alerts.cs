using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALRS.Models
{
    public class Alerts
    {
        public int Id { get; set; }
        public string? VictimName { get; set; }
        public int VictimAge { get; set; }
        public string? CrimeLocation { get; set; }
        public string? CrimeDate { get; set; }
        public int CrimeStatus { get; set; }

        public KidnapperDetailsAlerts? KidnapperDetailsAlerts { get; set; }
    }

    public class KidnapperDetailsAlerts
    {
        public int Id { get; set; }
        public string KidnapperName { get; set; }
        public int KidnapperAge { get; set; }
        public string KidnapperSex { get; set; }
        public string KidnapperLook { get; set; }
        public string KidnapperVehicle { get; set; }

        public int AlertsId { get; set; }
        public Alerts Alerts { get; set; }
    }
}

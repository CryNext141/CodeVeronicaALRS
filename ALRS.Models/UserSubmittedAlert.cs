using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALRS.Models
{
    public class UserSubmittedAlert
    {
        public int Id { get; set; }
        public string? VictimName { get; set; }
        public int? VictimAge { get; set; }
        public string CrimeLocation { get; set; }
        public string CrimeDate { get; set; }
    }


    public class KidnapperDetails
    {
        public int Id { get; set; }
        public string? KidnapperName { get; set; }
        public int? KidnapperAge { get; set; }
        public string KidnapperSex { get; set; }
        public string? KidnapperClothes { get; set; }
        public string? KidnapperVehicle { get; set; }
    }
}

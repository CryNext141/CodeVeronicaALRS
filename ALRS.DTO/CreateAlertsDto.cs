using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALRS.DTO
{
    public class CreateAlertDto
    {
        public string? VictimName { get; set; }
        public int VictimAge { get; set; }
        public string? CrimeLocation { get; set; }
        public string? CrimeDate { get; set; }
        public int CrimeStatus { get; set; }

        public string? KidnapperName { get; set; }
        public int? KidnapperAge { get; set; }
        public string? KidnapperSex { get; set; }
        public string? KidnapperLook { get; set; }
        public string? KidnapperVehicle { get; set; }
    }

}

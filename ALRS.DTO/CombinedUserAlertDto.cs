using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALRS.DTO
{
    public class CombinedUserAlertDto
    {
        public int AlertsId { get; set; }
        public UserAlertDto UserAlert { get; set; }
        public KidnapperDto Kidnapper { get; set; }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALRS.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

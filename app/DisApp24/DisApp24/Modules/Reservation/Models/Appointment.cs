using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisApp24
{
    public class Appointment
    {
        public string Key { get; set; }
        public string Resource { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Date { get; set; }
        public string TimeSlot { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonIgnore]
        public Color StatusColor
        {
            get
            {
                return Status switch
                {
                    "Pending" => Colors.Yellow,
                    "Confirmed" => Colors.Green,
                    "Declined" => Colors.Red,
                    _ => Colors.Gray,
                };
            }
        }
    }

}

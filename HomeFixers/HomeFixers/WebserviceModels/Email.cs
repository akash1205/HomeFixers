using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeFixers.WebserviceModels
{
    public class Email
    {
        public string customerEmail { get; set; }
        public string serviceEmail { get; set; }
        public string serviceName { get; set; }
        public int bookingID { get; set; }
        public DateTime bookingDate { get; set; }
        public string slot { get; set; }
    }
}
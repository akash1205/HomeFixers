using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeFixers.WebserviceModels
{
    public class Person
    {
        public long ID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public int CriminalRecords { get; set; }
        public int DrivingTickets { get; set; }
    }
}
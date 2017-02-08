using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime scheduledate { get; set; }
        public bool Sch_slot1 { get; set; }
        public bool Sch_slot2 { get; set; }
        public bool Sch_slot3 { get; set; }
        public bool Sch_slot4 { get; set; }

        public virtual int ServiceproviderID { get; set; }
        public virtual ServiceProvider serviceprovider { get; set; }
    }
}
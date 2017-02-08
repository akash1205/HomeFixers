using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class Service
    {
        public int id { get; set; }

        public string ServiceName { get; set; }

        public virtual List<ServiceProvider> serviceprovider { get; set; }
    }
}
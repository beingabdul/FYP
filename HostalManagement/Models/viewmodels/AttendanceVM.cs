using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HostalManagement.Models.viewmodels
{
    public class AttendanceVM
    {

        public int ID { get; set; }
        public int StdID { get; set; }
        public Nullable<System.DateTime> CheckIn { get; set; }
        public Nullable<System.DateTime> CheckOut { get; set; }
        public bool Status { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HostalManagement.Models.viewmodels
{
    public class AttendanceVM
    {

        public int ID { get; set; }
        public int StdID { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public bool Status { get; set; }
        public string Date { get; set; }
    }
}
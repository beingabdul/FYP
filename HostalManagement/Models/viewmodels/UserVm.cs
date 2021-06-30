using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HostalManagement.Models.viewmodels
{
    public class UserVm
    {
        public int RegistrationId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string FatherRank { get; set; }
        public string CNIC { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FamilyNo { get; set; }
        public string BloodGroup { get; set; }
        public string HomeAddress { get; set; }
        public string Institute { get; set; }
        public string Degree { get; set; }
        public string DegreeSession { get; set; }
        public string Convience { get; set; }
        public string VehicleNo { get; set; }
        public string LicenseNo { get; set; }
        public Nullable<int> Catagory { get; set; }
        public Nullable<int> UserRoleId { get; set; }
        public string Photo { get; set; }
    }
}
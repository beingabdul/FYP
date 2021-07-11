using System;

namespace HostalManagement.Controllers
{
    public class ParentVM
    {
        public int RegistrationId { get; set; }
        public string Name { get; set; }
        public string CNIC { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string StudentID { get; set; }
        public Nullable<int> UserRoleId { get; set; }
    }
}
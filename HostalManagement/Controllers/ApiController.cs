using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HostalManagement.Models;

namespace HostalManagement.Controllers
{
    public class ApiForMobileController : GlobalController

    {
        private HostalManagementDB01Entities db = new HostalManagementDB01Entities();
        // GET: Api
        [HttpGet]
        public JsonResult GetStudent(string email, string password)
        {
            try
            {
                Registration u = db.Registrations.FirstOrDefault(x => x.Email == email && x.Password == password);
                if (u != null)
                {
                    return Json(u, JsonRequestBehavior.AllowGet);
                }
                return Json("Not Found", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw ex;
            }
        }
    }
}
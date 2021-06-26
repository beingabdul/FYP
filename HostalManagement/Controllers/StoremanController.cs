using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HostalManagement.Models;


namespace HostalManagement.Controllers
{
    public class StoremanController : Controller
    {
        private HostalManagementDB01Entities db = new HostalManagementDB01Entities();

        public ActionResult Index()
        {
            var messings = db.Messings.Where(a => a.Status == false).ToList();
            ViewBag.total = messings.Count();
            return View();
        }

        public ActionResult CheckOrder()
        {
            var messings = db.Messings.Where(a => a.Status == false).Include(m => m.FoodList).Include(m => m.MealType).Include(m => m.Registration).Include(m => m.Weekday);
            return View(messings.ToList());
        }

        [HttpPost]
        public JsonResult ReadytoCook(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                int setid = Convert.ToInt32(id);
                db.getReadytoCook(setid);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}
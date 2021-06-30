using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HostalManagement.Models;
using HostalManagement.Models.viewmodels;

namespace HostalManagement.Controllers
{
    public class StudentController : GlobalController
    {
        private readonly HostalManagementDB01Entities db = new HostalManagementDB01Entities();

        public ActionResult Index()
        {
            if (!Authenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (SiteUser.UserRoleId == 2)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public ActionResult Ordernow()
        {
            if (!Authenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            if (SiteUser.UserRoleId == 2)
            {
                var day = DateTime.Now.DayOfWeek.ToString();
                ViewBag.WeekdayId = new SelectList(db.Weekdays.Where(a => a.Name == day), "WeekdayId", "Name");
                ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }
        public JsonResult getFoodPlan(int id)
        {
            var day = DateTime.Now.DayOfWeek.ToString();
            Weekday DayId = db.Weekdays.FirstOrDefault(a => a.Name == day);
            int getDayId = DayId.WeekdayId;
            FoodList WeekDayId = db.FoodLists.FirstOrDefault(a => a.WeekdayId == getDayId);
            var getWeekDayId = WeekDayId.WeekdayId;
            var listOfgetDepartment = db.FoodLists.Where(x => x.MealTypeId == id && x.WeekdayId == getWeekDayId && x.Status == true)
                  .Select(s => new SelectListItem
                  {
                      Value = s.FoodListId.ToString(),
                      Text = s.Name + "  Price: " + s.Price
                  });
            return Json(new SelectList(listOfgetDepartment, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        [HttpPost]
        public ActionResult Ordernow(Messing m)
        {
            try
            {
                DateTime dt = DateTime.Now;
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;
                var orderdate = day + "-" + month + "-" + year;
                //m.OrderDate = Convert.ToString(orderdate);
                m.OrderDate = Convert.ToString(DateTime.Now);
                m.MonthId = month;
                m.Status = false;
                m.Hostory = false;
                m.RegistrationId = SiteUser.RegistrationId;
                FoodList f = db.FoodLists.FirstOrDefault(a => a.FoodListId == m.FoodListId);
                m.Price = Convert.ToInt32(f.Price);
                db.Messings.Add(m);
                db.SaveChanges();

                var nowday = DateTime.Now.DayOfWeek.ToString();
                ViewBag.WeekdayId = new SelectList(db.Weekdays.Where(a => a.Name == nowday), "WeekdayId", "Name");
                ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name");
                var name = SiteUser.Name;
                TempData["msg"] = String.Format("Dear " + name + " Your Order is Done, Please wait!");
                return RedirectToAction("Ordernow");
            }
            catch 
            {
                return View();
            }
        }
        public ActionResult Foodlist()
        {
            if (!Authenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            if (SiteUser.UserRoleId == 2)
            {
                var foodLists = db.FoodLists.Include(f => f.MealType).Include(f => f.Weekday).OrderBy(f=>f.WeekdayId);
                return View(foodLists.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public ActionResult CurrentStatus()
        {
            DateTime dt = DateTime.Now;
            int mid = dt.Month;
            var rid =  SiteUser.RegistrationId;
            //Month month = new Month();
            //ViewBag.currentmonth = month.Name.Where();
            Month m = db.Months.FirstOrDefault(a => a.MonthId == mid);
            ViewBag.mname =   m.Name;
            ViewBag.list = db.getMonthyReportOrderByStudentSingle(rid, mid).ToList();
            return View();
        }
        #region Apis
        public JsonResult GetMenu()
        {
            List<FoodListsVM> foodlist = new List<FoodListsVM>();
            foodlist = db.FoodLists.Select(p=> new FoodListsVM() {
                MealTypeId = p.MealType.MealTypeId,
                MealType = p.MealType.Name,
                Weekday=p.Weekday.Name,
                WeekdayId=p.Weekday.WeekdayId,
                MealPrice=p.Price,
                MealName=p.Name
            }).OrderBy(f => f.WeekdayId).ToList();
            return Json(foodlist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DailyMessing(int student_id)
        {
            DateTime dt = DateTime.Now;
            int mid = dt.Month;
            Month m = db.Months.FirstOrDefault(a => a.MonthId == mid);
            ViewBag.mname = m.Name;
            var list = db.getMonthyReportOrderByStudentSingle(student_id, mid).ToList();
            foreach (var item in list)
            {
                if (item.Status==false)
                {
                    list.Remove(item);
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAttendance(int student_id)
        {
            List<AttendanceVM> attandance = new List<AttendanceVM>();
            attandance = db.Attendances.Where(at => at.StdID == student_id).Select(a=> new AttendanceVM() { 
            Date=a.Date,
            CheckIn=a.CheckIn,
            CheckOut=a.CheckOut
            }).ToList();
            return Json(attandance, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
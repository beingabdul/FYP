using HostalManagement.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HostalManagement.Controllers
{
    public class ExtraController : GlobalController
    {
        private HostalManagementDB01Entities db = new HostalManagementDB01Entities();


        public ActionResult Ordernow()
        {
            var day = DateTime.Now.DayOfWeek.ToString();
            ViewBag.WeekdayId = new SelectList(db.Weekdays.Where(a => a.Name == day), "WeekdayId", "Name");
            ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name");
            return View();
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
                      Text = s.Name + "-- Price: " + s.Price
                  });
            return Json(new SelectList(listOfgetDepartment, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public ActionResult Ordernow(Messing m)
        {
            DateTime dt = DateTime.Now;
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            var orderdate = day + "-" + month + "-" + year;
            m.OrderDate = Convert.ToString(orderdate);
            m.MonthId = month;
            m.Status = false;
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

        [HttpGet]
        public ActionResult getReport()
        {
            var listOfgetDepartment = db.Registrations
                  .Select(s => new SelectListItem
                  {
                      Value = s.RegistrationId.ToString(),
                      Text = s.Name + "--" + s.CNIC
                  });
            ViewBag.RegistrationId = new SelectList(listOfgetDepartment, "Value", "Text");
            ViewBag.MonthId = new SelectList(db.Months, "MonthId", "Name");

            return View();
        }

        public ActionResult getReport(Bill b)
        {
            var rid = b.RegistrationId;
            var mid = Convert.ToInt32(b.MonthId);
            ViewBag.getMonthlyReport = db.getMonthyReportOrderByStudent(rid, mid).ToList();
            var gmr = ViewBag.getMonthlyReport;
            var gmt = db.Messings.Where(a => a.RegistrationId == rid && a.MonthId == mid).Sum(x => x.Price);
            //ViewBag.getMonthyTotal = db.getMonthyTotal(rid, mid).ToList();
            //var gmt = ViewBag.getMonthyTotal;
            //ViewBag.getTotal = db.getTotal01(rid).ToList();
            //var gt = ViewBag.getTotal;

            b.FoodBill = gmt;
            b.Laundry = 1000;
            b.Internet = 200;
            b.HouseRent = 3500;

            db.Bills.Add(b);
            db.SaveChanges();

            ViewBag.MealTypeId = new SelectList(db.MealTypes, "RegistrationId", "Name");
            ViewBag.WeekdayId = new SelectList(db.Weekdays, "MonthId", "Name");
            return View();
        }

        public ActionResult list()
        {
            ViewBag.getMonthlyReport = db.getMonthyReportOrderByStudent(3, 6).ToList();
            ViewBag.list = db.Bills.Where(a => a.RegistrationId == 3 && a.MonthId == 6).ToList();
            var gmt = db.Messings.Where(a => a.RegistrationId == 3 && a.MonthId == 6).Sum(x => x.Price);
            return View();
        }

        //testing
        public ActionResult Index()
        {
            var sday = DateTime.Now.DayOfWeek.ToString();
            var smonth = DateTime.Now.Month.ToString();
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(smonth));

            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            string strMonthName = mfi.GetMonthName(8).ToString();

            DateTime dt = DateTime.Now;
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            return View();
        }

        public ActionResult abc()
        {
            int id = 1;
            var day = DateTime.Now.DayOfWeek.ToString();
            Weekday wd = db.Weekdays.FirstOrDefault(a => a.Name == day);
            int dayid = wd.WeekdayId;
            //DateTime dt = DateTime.Now;
            //int dayid = dt.Day;
            FoodList wdd = db.FoodLists.FirstOrDefault(a => a.WeekdayId == dayid);
            var n = wdd.WeekdayId;
            //var n = new SelectList(db.FoodLists.Where(a => a.WeekdayId == dayid), "WeekdayId");
            var listOfgetDepartment = db.FoodLists.Where(x => x.MealTypeId == id && x.WeekdayId == n).ToList();
            return View();
        }

    }
}
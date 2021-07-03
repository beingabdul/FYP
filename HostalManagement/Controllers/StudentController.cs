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

        #region WebApp

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
                var foodLists = db.FoodLists.Include(f => f.MealType).Include(f => f.Weekday).OrderBy(f => f.WeekdayId);
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
            var rid = SiteUser.RegistrationId;
            //Month month = new Month();
            //ViewBag.currentmonth = month.Name.Where();
            Month m = db.Months.FirstOrDefault(a => a.MonthId == mid);
            ViewBag.mname = m.Name;
            ViewBag.list = db.getMonthyReportOrderByStudentSingle(rid, mid).ToList();
            return View();
        }
        #endregion


        #region Apis
        public JsonResult GetMenu()
        {
            try
            {
                List<FoodListsVM> foodlist = new List<FoodListsVM>();
                foodlist = db.FoodLists.Select(p => new FoodListsVM()
                {
                    MealTypeId = p.MealType.MealTypeId,
                    MealType = p.MealType.Name,
                    Weekday = p.Weekday.Name,
                    WeekdayId = p.Weekday.WeekdayId,
                    MealPrice = p.Price,
                    MealName = p.Name
                }).OrderBy(f => f.WeekdayId).ToList();
                return Json(foodlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
                throw ex;
            }
        }
        public JsonResult DailyMessing(int student_id)
        {
            try
            {
                DateTime dt = DateTime.Now;
                int mid = dt.Month;
                Month m = db.Months.FirstOrDefault(a => a.MonthId == mid);
                ViewBag.mname = m.Name;
                var list = db.getMonthyReportOrderByStudentSingle(student_id, mid).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
                throw ex;
            }

        }
        public JsonResult GetAttendance(int student_id)
        {
            try
            {
                List<AttendanceVM> attandance = new List<AttendanceVM>();
                attandance = db.Attendances.Where(at => at.StdID == student_id).Select(a => new AttendanceVM()
                {
                    Date = a.Date.ToString(),
                    CheckIn = a.CheckIn.ToString(),
                    CheckOut = a.CheckOut.ToString()
                }).ToList();
                return Json(attandance, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
                throw ex;
            }

        }

        [HttpGet]
        public JsonResult PlaceOrder()
        {
            try
            {
                PlaceOrder order = new PlaceOrder();
                var day = DateTime.Now.DayOfWeek.ToString();
                order.MealType = db.MealTypes.Select(m => new MealTypeVM{
                    MealTypeId=m.MealTypeId,
                    Name=m.Name 
                }).ToList();
                order.Weekday =  db.Weekdays.Where(a => a.Name == day).Select(m => new WeekdayVM{
                    WeekdayId = m.WeekdayId, 
                    Name = m.Name 
                }).FirstOrDefault();
                order.FoodList = db.FoodLists.Where(f => f.WeekdayId == order.Weekday.WeekdayId).Select(n => new FoodListVM{ 
                    FoodListId = n.FoodListId, 
                    Name = n.Name, 
                    Price = n.Price,
                    MealTypeId= (int)n.MealTypeId
                }).ToList();
                return Json(order, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult PlaceOrder(int student_id, int FoodListId, int MealTypeId, int WeekdayId)
        {
            try
            {
                Messing m = new Messing();
                DateTime dt = DateTime.Now;
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;
                var orderdate = day + "-" + month + "-" + year;
                //m.OrderDate = Convert.ToString(orderdate);
                m.OrderDate = Convert.ToString(DateTime.Now);
                m.MonthId = month;
                m.WeekdayId = WeekdayId;
                m.MealTypeId = MealTypeId;
                m.Status = false;
                m.Hostory = false;
                m.RegistrationId = student_id;
                m.FoodListId = FoodListId;
                FoodList f = db.FoodLists.FirstOrDefault(a => a.FoodListId == FoodListId);
                m.Price = Convert.ToInt32(f.Price);
                db.Messings.Add(m);
                if (db.SaveChanges() > 0)
                {
                    return Json("succes", JsonRequestBehavior.AllowGet);
                }
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw ex;
            }
        }
        #endregion
    }
}
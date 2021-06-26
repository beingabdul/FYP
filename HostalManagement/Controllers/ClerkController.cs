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
    public class ClerkController : Controller
    {

        private HostalManagementDB01Entities db = new HostalManagementDB01Entities();

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult StudentCreate()
        {
            ViewBag.Catagory = new SelectList(db.Catagories, "CatagoryId", "Name");
            ViewBag.UserRoleId = new SelectList(db.UserRoles.Where(a => a.UserRoleId == 2), "UserRoleId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentCreate([Bind(Include = "RegistrationId,Name,FatherName,FatherRank,CNIC,ContactNo,Email,Password,FamilyNo,BloodGroup,HomeAddress,Institute,Degree,DegreeSession,Convience,VehicleNo,LicenseNo,Catagory,UserRoleId")] Registration registration)
        {
            if (ModelState.IsValid)
            {
                db.Registrations.Add(registration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Catagory = new SelectList(db.Catagories, "CatagoryId", "Name", registration.Catagory);
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }

        public ActionResult StudentList()
        {
            var registrations = db.Registrations.Where(a=>a.UserRoleId == 2).Include(r => r.Catagory1).Include(r => r.UserRole);
            return View(registrations.ToList());
        }

        public ActionResult StudentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registration registration = db.Registrations.Find(id);
            if (registration == null)
            {
                return HttpNotFound();
            }
            ViewBag.Catagory = new SelectList(db.Catagories, "CatagoryId", "Name", registration.Catagory);
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentEdit([Bind(Include = "RegistrationId,Name,FatherName,FatherRank,CNIC,ContactNo,Email,Password,FamilyNo,BloodGroup,HomeAddress,Institute,Degree,DegreeSession,Convience,VehicleNo,LicenseNo,Catagory,UserRoleId")] Registration registration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Catagory = new SelectList(db.Catagories, "CatagoryId", "Name", registration.Catagory);
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }
        public ActionResult StudentDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registration stddetail = db.Registrations.Find(id);
            if (stddetail == null)
            {
                return HttpNotFound();
            }
            return View(stddetail);
        }
        

        public ActionResult StudentDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Registration registration = db.Registrations.Find(id);

            if (registration == null)
            {
                return HttpNotFound();
            }
            return View(registration);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("StudentDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult StudentDelete(int id)
        {
            if (true)
            {

                //Bill bill = db.Bills.Find(id);
                //if(id == bill.RegistrationId)
                //db.Bills.Remove(bill);
                Registration registration = db.Registrations.Find(id);
                db.Registrations.Remove(registration);
                db.SaveChanges();
                return RedirectToAction("StudentList");
            }

        }
        public ActionResult MenuList()
        {
            var foodLists = db.FoodLists.Include(f => f.MealType).Include(f => f.Weekday);
            return View(foodLists.ToList());
        }

        public ActionResult MenuCreate()
        {
            ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name");
            ViewBag.WeekdayId = new SelectList(db.Weekdays, "WeekdayId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MenuCreate([Bind(Include = "FoodListId,Name,Price,Status,WeekdayId,MealTypeId")] FoodList foodList)
        {
            if (ModelState.IsValid)
            {
                db.FoodLists.Add(foodList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name", foodList.MealTypeId);
            ViewBag.WeekdayId = new SelectList(db.Weekdays, "WeekdayId", "Name", foodList.WeekdayId);
            return View(foodList);
        }

        public ActionResult MenuEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodList foodList = db.FoodLists.Find(id);
            if (foodList == null)
            {
                return HttpNotFound();
            }
            ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name", foodList.MealTypeId);
            ViewBag.WeekdayId = new SelectList(db.Weekdays, "WeekdayId", "Name", foodList.WeekdayId);
            return View(foodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MenuEdit([Bind(Include = "FoodListId,Name,Price,Status,WeekdayId,MealTypeId")] FoodList foodList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foodList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name", foodList.MealTypeId);
            ViewBag.WeekdayId = new SelectList(db.Weekdays, "WeekdayId", "Name", foodList.WeekdayId);
            return View(foodList);
        }


        [HttpGet]
        public ActionResult GenerateBill()
        {
            var listOfgetDepartment = db.Registrations.Where(a => a.UserRoleId == 2)
                  .Select(s => new SelectListItem
                  {
                      Value = s.RegistrationId.ToString(),
                      Text = s.Name + "--" + s.CNIC
                  });
            ViewBag.RegistrationId = new SelectList(listOfgetDepartment, "Value", "Text");
            ViewBag.MonthId = new SelectList(db.Months, "MonthId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult GenerateBill(Bill b)
        {
            DateTime dt = DateTime.Now;
            BillAudit BA = new BillAudit();
            int day = dt.Day;
            if (day == 1 || day == 9)
            {
                var rid = b.RegistrationId;
                var mid = Convert.ToInt32(b.MonthId);
                Messing check = db.Messings.FirstOrDefault(a => a.RegistrationId == rid && a.MonthId == mid && a.Hostory == false);
                if (check != null)
                {
                    var totalfoodbill = db.Messings.Where(a => a.RegistrationId == rid && a.MonthId == mid && a.Hostory == false).ToList().Sum(x => x.Price);
                    if (totalfoodbill != null)
                    {
                        var clearhistory = db.Messings.Where(a => a.RegistrationId == rid && a.MonthId == mid && a.Hostory == false).ToList();
                        foreach (var i in clearhistory)
                        {
                            i.Hostory = true;
                            db.SaveChanges();

                        }
                        Registration getUser = db.Registrations.FirstOrDefault(a => a.RegistrationId == rid);
                        var z = getUser.Catagory;
                        if (z == 1) //Soldier
                        {
                            b.HouseRent = 500;
                        }
                        else if (z == 2) //JCO
                        {
                            b.HouseRent = 1000;
                        }
                        else if (z == 3) //Officer
                        {
                            b.HouseRent = 3000;
                        }
                        b.FoodBill = totalfoodbill;
                        b.Internet = 200;
                        b.Laundry = 1000;
                        db.Bills.Add(b);

                        if (db.SaveChanges() > 0)
                        {
                            var bid = b.Billid;
                            foreach (var i in clearhistory)
                            {
                                BA.Billid = bid;
                                BA.MessingId = i.MessingId;
                                db.BillAudits.Add(BA);
                                db.SaveChanges();
                            }

                        }
                        TempData["msg"] = String.Format("Bill Generate !");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["msg"] = String.Format("No Order History!");
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    TempData["msg"] = String.Format("NO order History of this request!");
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["msg"] = String.Format("Must generate report 1,2 Date .....!");
                return RedirectToAction("Index");
            }
        }

        public ActionResult GenerateBillReport()
        {
            return View(db.Bills.ToList());
        }

        public ActionResult BillReport(int id)
        {
            Bill b = db.Bills.FirstOrDefault(a => a.Billid == id);
            var rid = b.RegistrationId;
            var mid = Convert.ToInt32(b.MonthId);
            ViewBag.getMonthlyReport = db.getMonthyReportOrderByStudent02(id).ToList();
            //ViewBag.getMonthlyReport = db.getMonthyReportOrderByStudent01(rid, mid).ToList();
            ViewBag.list = db.Bills.Where(a => a.Billid == id && a.RegistrationId == rid && a.MonthId == mid).ToList();
            Month list = db.Months.FirstOrDefault(a => a.MonthId == mid);
            ViewBag.Month = list.Name;
            ViewBag.Total = db.Bills.Where(a => a.Billid == id).Sum(a => a.FoodBill + a.HouseRent + a.Internet + a.Laundry);
            ViewBag.date = DateTime.Now;
            var bills = db.Bills.Include(x => x.Month).Include(x => x.Registration).ToList(); ;
            return View(bills);
        }


    }
}
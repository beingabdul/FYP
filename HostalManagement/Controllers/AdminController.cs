using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HostalManagement.Models;
using Newtonsoft.Json;

namespace HostalManagement.Controllers
{
    public class AdminController : GlobalController
    {
        private readonly HostalManagementDB01Entities db = new HostalManagementDB01Entities();
        private static string ServiceKey = ConfigurationManager.AppSettings["FaceServiceKey"];
        public ActionResult Index()
        {
            return View();
        }
        #region Bill
        [HttpGet]
        public ActionResult GenerateBill()
        {
            var listOfgetDepartment = db.Registrations.Where(a => a.UserRoleId == 2)
                  .Select(s => new SelectListItem
                  {
                      Value = s.RegistrationId.ToString(),
                      Text = s.Name + "--" + s.CNIC
                  });

            DateTime dt = DateTime.Now;
            int month = dt.Month;
            ViewBag.RegistrationId = new SelectList(listOfgetDepartment, "Value", "Text");
            ViewBag.MonthId = new SelectList(db.Months.Where(a => a.MonthId < month), "MonthId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult GenerateBill(Bill b)
        {
            DateTime dt = DateTime.Now;
            BillAudit BA = new BillAudit();
            int day = dt.Day;
            int month = dt.Month;

            var rid = b.RegistrationId;
            var mid = Convert.ToInt32(b.MonthId);
            if (day <= 10)
            {
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
                        TempData["msg"] = String.Format("Bill Generated!");
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
                TempData["msg"] = String.Format("You cannot generate bill after 10 of every month");
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
            ViewBag.list = db.Bills.Where(a => a.Billid == id && a.RegistrationId == rid && a.MonthId == mid).ToList();
            Month list = db.Months.FirstOrDefault(a => a.MonthId == mid);
            ViewBag.Month = list.Name;
            ViewBag.Total = db.Bills.Where(a => a.Billid == id).Sum(a => a.FoodBill + a.HouseRent + a.Internet + a.Laundry);
            ViewBag.date = DateTime.Now;
            var bills = db.Bills.Include(x => x.Month).Include(x => x.Registration).ToList(); ;
            return View(bills);
        }
        //GET Student bills
        public ActionResult ReceiveStdBills()
        {
            var listOfgetDepartment = db.Registrations.Where(a => a.UserRoleId == 2)
                   .Select(s => new SelectListItem
                   {
                       Value = s.RegistrationId.ToString(),
                       Text = s.Name + "--" + s.CNIC
                   });

            DateTime dt = DateTime.Now;
            int month = dt.Month;
            ViewBag.RegistrationId = new SelectList(listOfgetDepartment, "Value", "Text");
            return View();
        }
        [HttpPost]
        public ActionResult ReceiveStdBills(Registration r)
        {
            try
            {
                Bill bill = db.Bills.FirstOrDefault(x => x.RegistrationId == r.RegistrationId);
                if (bill != null)
                {
                    BillAudit billaudit = db.BillAudits.FirstOrDefault(x => x.Billid == bill.Billid);
                    if (billaudit != null)
                    {
                        Messing messing = db.Messings.FirstOrDefault(x => x.MessingId == billaudit.MessingId);
                        if (messing != null)
                        {

                            db.Messings.Remove(messing);
                        }
                        db.BillAudits.Remove(billaudit);
                    }
                    db.Bills.Remove(bill);
                    db.SaveChanges();
                    TempData["msg"] = String.Format("Bill Received!");
                    }
                    return RedirectToAction("ReceiveStdBills");
                }
            catch (Exception ex)
            {
                TempData["msg"] = String.Format("error");
                return RedirectToAction("ReceiveStdBills");
                throw ex;
            }
        }
        #endregion
        #region student
        public ActionResult StudentCreate()
        {
            ViewBag.Catagory = new SelectList(db.Catagories, "CatagoryId", "Name");
            ViewBag.UserRoleId = new SelectList(db.UserRoles.Where(a => a.UserRoleId == 2), "UserRoleId", "Name");
            return View();
        }

        [HttpPost, ActionName("StudentCreate")]
        public ActionResult StudentCreate(Registration reg)
        {
            string status;
            try
            {

                if (ModelState.IsValid)
                {
                    Registration r = db.Registrations.FirstOrDefault(x => x.CNIC == reg.CNIC || x.Email == reg.Email);
                    if (r != null)
                    {
                        status = "yes";
                        return Content(status);
                    }
                    reg.UserRoleId = 2;
                    db.Registrations.Add(reg);
                    db.SaveChanges();

                    status = "success";
                    return Content(status);
                }

                else
                {
                    status = "error";
                    return Content(status);
                }


            }
            catch (Exception)
            {
                status = "error";
                return Content(status);
            }
        }
        public ActionResult StudentList()
        {
            var registrations = db.Registrations.Where(a => a.UserRoleId == 2).Include(r => r.Catagory1).Include(r => r.UserRole);
            return View(registrations.ToList());
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
                return RedirectToAction("StudentList");
            }
            ViewBag.Catagory = new SelectList(db.Catagories, "CatagoryId", "Name", registration.Catagory);
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }

        public ActionResult StudentDelete(int? id)
        {

            Bill bill = db.Bills.FirstOrDefault(x => x.RegistrationId == id);
            Messing messing = db.Messings.FirstOrDefault(xx => xx.RegistrationId == id);

            if (bill != null && messing != null)
            {
                TempData["msg"] = string.Format("Student have unpaid bills..! Clear Bills First");
                return RedirectToAction("ReceiveStdBills");
            }
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
        public ActionResult StudentDelete(int id)
        {
            if (true)
            {


                Registration registration = db.Registrations.Find(id);
                db.Registrations.Remove(registration);
                db.SaveChanges();
                return RedirectToAction("StudentList");
            }

        }
        #endregion student
        #region Clerk
        //Get

        public ActionResult ClerkCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ClerkCreate(Registration reg)
        {
            string status;
            try
            {

                if (ModelState.IsValid)
                {
                    Registration r = db.Registrations.FirstOrDefault(x => x.CNIC == reg.CNIC || x.Email == reg.Email);
                    if (r != null)
                    {
                        status = "yes";
                        return Content(status);
                    }
                    reg.UserRoleId = 3;
                    db.Registrations.Add(reg);
                    db.SaveChanges();

                    status = "success";
                    return Content(status);
                }

                else
                {
                    status = "error";
                    return Content(status);
                }


            }
            catch (Exception)
            {
                status = "error";
                return Content(status);
            }

        }
        public ActionResult ClerkList()
        {
            var registrations = db.Registrations.Where(a => a.UserRoleId == 3).Include(r => r.Catagory1).Include(r => r.UserRole);
            return View(registrations.ToList());
        }
        //Get Clerk
        public ActionResult ClerkEdit(int? id)
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
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }
        //POST Clerk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClerkEdit([Bind(Include = "RegistrationId,Name,FatherName,CNIC,ContactNo,Email,Password,BloodGroup,HomeAddress,UserRoleId")] Registration registration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClerkList");
            }
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }
        public ActionResult ClerkDetail(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registration clkdetail = db.Registrations.Find(id);
            if (clkdetail == null)
            {

                return HttpNotFound();
            }
            return View(clkdetail);
        }
        public ActionResult ClerkDelete(int? id)
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

        // POST: Clerk/Delete/5
        [HttpPost, ActionName("ClerkDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult ClerkDelete(int id)
        {
            if (true)
            {

                //Bill bill = db.Bills.Find(id);
                //if(id == bill.RegistrationId)
                //db.Bills.Remove(bill);
                Registration registration = db.Registrations.Find(id);
                db.Registrations.Remove(registration);
                db.SaveChanges();
                return RedirectToAction("ClerkList");
            }

        }
        #endregion
        #region Storeman
        public ActionResult StoremanCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StoremanCreate(Registration reg)
        {
            string status;
            try
            {

                if (ModelState.IsValid)
                {
                    Registration r = db.Registrations.FirstOrDefault(x => x.CNIC == reg.CNIC || x.Email == reg.Email);
                    if (r != null)
                    {
                        status = "yes";
                        return Content(status);
                    }
                    reg.UserRoleId = 4;
                    db.Registrations.Add(reg);
                    db.SaveChanges();

                    status = "success";
                    return Content(status);
                }

                else
                {
                    status = "error";
                    return Content(status);
                }


            }
            catch (Exception)
            {
                status = "error";
                return Content(status);
            }

        }
        public ActionResult StoremanList()
        {
            var registrations = db.Registrations.Where(a => a.UserRoleId == 4).Include(r => r.Catagory1).Include(r => r.UserRole);
            return View(registrations.ToList());
        }
        //Get StoremanEdit
        public ActionResult StoremanEdit(int? id)
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
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }
        //POST StoremanEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoremanEdit(Registration registration)
        {

            if (ModelState.IsValid)
            {
                db.Entry(registration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StoremanList");
            }
            ViewBag.UserRoleId = new SelectList(db.UserRoles, "UserRoleId", "Name", registration.UserRoleId);
            return View(registration);
        }
        public ActionResult StoremanDetail(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registration stmdetail = db.Registrations.Find(id);
            if (stmdetail == null)
            {

                return HttpNotFound();
            }
            return View(stmdetail);
        }
        public ActionResult StoremanDelete(int? id)
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

        // POST: Storeman/Delete/5
        [HttpPost, ActionName("StoremanDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult StoremanDelete(int id)
        {
            if (true)
            {

                //Bill bill = db.Bills.Find(id);
                //if(id == bill.RegistrationId)
                //db.Bills.Remove(bill);
                Registration registration = db.Registrations.Find(id);
                db.Registrations.Remove(registration);
                db.SaveChanges();
                return RedirectToAction("StoremanList");
            }

        }
        #endregion

        public ActionResult AttendanceRegisteration()
        {
            ViewBag.stds = db.Registrations.Where(a => a.UserRoleId == 2).ToList();
            return View();
        }
        [HttpPost]
        public JsonResult AttendanceRegisteration(Registration re)
        {
            try
            {
                if (re.RegistrationId > 0 && re.Photo != "")
                {
                    string randomFileName = Guid.NewGuid().ToString().Substring(0, 10) + ".png";
                    String path = Server.MapPath("~/assets/images/Test/"); //Path
                    //Check if directory exist
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path); //Create directory if it doesn't exist
                    }
                    //if (re.Photo.Contains("jpeg"))
                    //{
                    //    randomFileName = Guid.NewGuid().ToString().Substring(0, 4) + ".jpg";
                    //}
                    //else if (re.Photo.Contains("png"))
                    //{
                    //    randomFileName = Guid.NewGuid().ToString().Substring(0, 4) + ".png";
                    //}
                    //else
                    //{
                    //    randomFileName = Guid.NewGuid().ToString().Substring(0, 4) + ".gif";
                    //}
                    // set the image path
                    string imgPath = Path.Combine(path, randomFileName);
                    string t = re.Photo.Substring(re.Photo.IndexOf(',') + 1);//remove data:image/png;base64, till ,                
                    string t2 = t.Remove(t.Length - 1, 1);
                    byte[] bytes = Convert.FromBase64String(t);
                    //Image image;
                    //using (MemoryStream ms = new MemoryStream(bytes))
                    //{
                    //    image = Image.FromStream(ms);
                    //}
                    System.IO.File.WriteAllBytes(imgPath, Convert.FromBase64String(t));
                    var user = db.Registrations.Where(res => res.RegistrationId == re.RegistrationId).FirstOrDefault();
                    user.Photo = imgPath;
                    db.Entry(user).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        return Json("success", JsonRequestBehavior.AllowGet);
                    }
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw ex;
            }

        }

        #region Parents
        public ActionResult ParentsList()
        {
            var registrations = db.Registrations.Where(a => a.UserRoleId == 5).Include(r => r.Catagory1).Include(r => r.UserRole);
            return View(registrations.ToList());
        }
        public ActionResult ParentsCreate()
        {

            ViewBag.Students = db.Registrations.Where(a => a.UserRoleId == 2).ToList();
            ViewBag.UserRoleId = new SelectList(db.UserRoles.Where(a => a.UserRoleId == 5), "UserRoleId", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult ParentsCreate(Registration reg)
        {
            string status;
            try
            {

                if (ModelState.IsValid)
                {
                    Registration r = db.Registrations.FirstOrDefault(x => x.CNIC == reg.CNIC || x.Email == reg.Email);
                    if (r != null)
                    {
                        status = "yes";
                        return Content(status);
                    }
                    reg.UserRoleId = 5;
                    db.Registrations.Add(reg);
                    db.SaveChanges();

                    status = "success";
                    return Content(status);
                }

                else
                {
                    status = "error";
                    return Content(status);
                }


            }
            catch (Exception)
            {
                status = "error";
                return Content(status);
            }
        }
        #endregion
    }
}

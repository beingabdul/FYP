using HostalManagement.Models;
using HostalManagement.Models.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HostalManagement.Controllers
{
    public class AccountController : GlobalController
    {
        private HostalManagementDB01Entities db = new HostalManagementDB01Entities();

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(Registration UserInfo)
        {
            try
            {
                TempData["msg"] = "You can login now.";
                return RedirectToAction("Login", "Account");
            }
            catch
            {
                TempData["Error"] = "Database Error";
                return View();
            }
        }

        /// <summary>
        /// get user login api
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        public JsonResult UserLogin(string email, string password, bool st)
        {
            UserVm user = new UserVm();
            try
            {
                user = db.Registrations.Where(x => x.Email == email && x.Password == password).Select(i=> new UserVm{ 
                RegistrationId=i.RegistrationId,
                Name=i.Name,
                FatherName=i.FatherName,
                FatherRank=i.FatherRank,
                FamilyNo=i.FamilyNo,
                CNIC=i.CNIC,
                ContactNo=i.ContactNo,
                Email=i.Email,
                BloodGroup=i.BloodGroup,
                HomeAddress=i.HomeAddress,
                Institute=i.Institute,
                Degree=i.Degree,
                DegreeSession=i.DegreeSession,
                Convience=i.Convience,
                LicenseNo=i.LicenseNo,
                VehicleNo=i.VehicleNo,
                Catagory=i.Catagory,
                Photo=i.Photo,
                UserRoleId=i.UserRoleId
                }).FirstOrDefault();
                
                if (user != null)
                {
                    if (user.UserRoleId == 5)
                    {
                        ParentVM p = new ParentVM
                        {
                            Name = user.Name,
                            Email = user.Email,
                            ContactNo = user.ContactNo,
                            StudentID = user.FamilyNo,
                            RegistrationId = user.RegistrationId,
                            CNIC = user.CNIC,
                            UserRoleId = user.UserRoleId
                        };
                        return Json(p, JsonRequestBehavior.AllowGet);
                    }
                    return Json(user, JsonRequestBehavior.AllowGet);
                }
                return Json("wrong email or password", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message,JsonRequestBehavior.AllowGet);
                throw ex;
            }

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Registration UserInfo)
        {
            string status = "error";
            try
            {
                Registration u = db.Registrations.FirstOrDefault(x => x.Email == UserInfo.Email && x.Password == UserInfo.Password);
                if (u != null)
                {
                    SiteUser = u;
                    var getusertype = SiteUser.UserRoleId;
                    Session["UserTypeActive"] = getusertype;
                    status = "UserIdActive";
                }
                else if (status == "error")
                {
                    TempData["msg"] = String.Format("Wrong Email and Password");
                    return RedirectToAction("Login", "Account");
                }
                if (status == "UserIdActive")
                {
                    //admin
                    if (u.UserRoleId == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    //student
                    else if (u.UserRoleId == 2)
                    {
                        return RedirectToAction("Index", "Student");
                    }
                    //clerk
                    else if (u.UserRoleId == 3)
                    {
                        return RedirectToAction("Index", "Clerk");
                    }
                    else if (u.UserRoleId == 4)
                    {
                        return RedirectToAction("Index", "Storeman");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch(Exception ex)
            {
                TempData["msg"] = ex.Message;//String.Format("Database Error");
                return RedirectToAction("Login", "Account");
                throw ex;
            }
        }
    }
}
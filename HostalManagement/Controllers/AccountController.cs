using HostalManagement.Models;
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
        public JsonResult Studentlogin(string email, string password, bool st)
        {
            try
            {
                Registration u = db.Registrations.FirstOrDefault(x => x.Email == email && x.Password == password);
                if (u != null)
                {
                    return Json(u, JsonRequestBehavior.AllowGet);
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
using HostalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HostalManagement.Controllers
{
    public class GlobalController : Controller
    {

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        //check session
        public Registration SiteUser //Get Site User
        {
            set
            {
                Session["ActiveUser"] = value;
            }
            get
            {
                Registration user = new Registration();
                user = Session["ActiveUser"] as Registration;
                return user;
            }
        }

        // to check user is Authenticated
        public bool Authenticated
        {
            get
            {
                Registration u = SiteUser;
                if (u != null)
                {
                    ViewBag.SiteUserName = u.RegistrationId;
                }
                return u != null;
            }
        }

        // is user id exist
        public int ActiveUserLocId
        {
            get
            {
                int id = 0;
                Registration u = SiteUser;
                if (u != null)
                {
                    id = u.RegistrationId;
                }
                return id;
            }
        }
    }
}
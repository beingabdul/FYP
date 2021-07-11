using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HostalManagement.Models;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace HostalManagement
{
    public static class HtmlHelperExtension
    {
        public static Registration ActiveUserInfo(this HtmlHelper html)
        {
            Registration u = new Registration();
            try
            {
                u = HttpContext.Current.Session["ActiveUser"] as Registration;
                return u;
            }
            catch(Exception ex) {
                return null;
                throw ex;
            }
        }
    }
}
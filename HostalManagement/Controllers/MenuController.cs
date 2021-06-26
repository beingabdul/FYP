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
    public class MenuController : Controller
    {
        private HostalManagementDB01Entities db = new HostalManagementDB01Entities();

        // GET: Menu
        public ActionResult Index()
        {
            var foodLists = db.FoodLists.Include(f => f.MealType).Include(f => f.Weekday);
            return View(foodLists.ToList());
        }

        // GET: Menu/Details/5
        public ActionResult Details(int? id)
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
            return View(foodList);
        }

        // GET: Menu/Create
        public ActionResult Create()
        {
            ViewBag.MealTypeId = new SelectList(db.MealTypes, "MealTypeId", "Name");
            ViewBag.WeekdayId = new SelectList(db.Weekdays, "WeekdayId", "Name");
            return View();
        }

        // POST: Menu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FoodListId,Name,Price,Status,WeekdayId,MealTypeId")] FoodList foodList)
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

        // GET: Menu/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Menu/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FoodListId,Name,Price,Status,WeekdayId,MealTypeId")] FoodList foodList)
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

        // GET: Menu/Delete/5
        public ActionResult Delete(int? id)
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
            return View(foodList);
        }

        // POST: Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodList foodList = db.FoodLists.Find(id);
            db.FoodLists.Remove(foodList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

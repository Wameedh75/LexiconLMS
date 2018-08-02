using LexiconLMS.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EntityState = System.Data.Entity.EntityState;

namespace LexiconLMS.Controllers
{
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activities
        public ActionResult Index(int? moduleId) {
            //var activities = from activity in db.Activities
            //    where activity.ModuleId == moduleId
            //    select activity;

            var activities = db.Activities.Where(a => a.ModuleId == moduleId);
            return View(activities.ToList());
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null) {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create(int id) {
            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name");
            //ViewBag.ModuleId = db.Modules.Find(moduleId);
            return View(new Activity{ModuleId = id,Starttime = DateTime.Now,EndTime = DateTime.Now});
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Starttime,EndTime,Description,Type,ModuleId")] Activity activity) {
            
            if (ModelState.IsValid) {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Details","Modules", new { id = activity.ModuleId });
            }

            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name", activity.ModuleId);
            ViewBag.ModuleId = db.Modules.Find(activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null) {
                return HttpNotFound();
            }
            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name", activity.ModuleId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Starttime,EndTime,Description,Type,ModuleId")] Activity activity) {

            if (ModelState.IsValid) {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details","Modules", new { id = activity.ModuleId });
            }
            //ViewBag.ModuleId = new SelectList(db.Modules, "Id", "Name", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null) {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Activity activity = db.Activities.Find(id);
            var moduleId = activity?.ModuleId;
            if (activity != null) {
                db.Documents.RemoveRange(db.Documents.Where(d => d.ActivityId == id));
                db.SaveChanges();
                db.Activities.Remove(activity);
                db.SaveChanges();
            }
            return RedirectToAction("Details","Modules", new { id = moduleId });
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

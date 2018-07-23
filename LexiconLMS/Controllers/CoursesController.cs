using LexiconLMS.Models;
using LexiconLMS.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EntityState = System.Data.Entity.EntityState;

namespace LexiconLMS.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Courses
        public ActionResult Index(string filterString = null)
        {
            var filteredCourses = db.Courses
                .Where(c => filterString == null || c.Name.Contains(filterString) || 
                            c.Description.Contains(filterString) 
                            //c.StartDate.ToString("yy-MM-dd").Contains(filterString) ||
                            //c.EndDate.ToString("yy-MM-dd").Contains(filterString)
                            )
                .Select(c => new CourseVeiwModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Description = c.Description,
                    CourseStudents = c.CourseStudents
                });
            return View(filteredCourses);
        }
        [Authorize]
        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }


        [Authorize(Roles = "teacher")]
        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,StartDate,EndDate,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }


        [Authorize(Roles = "teacher")]
        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "teacher")]
        public ActionResult Edit([Bind(Include = "Id,Name,StartDate,EndDate,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "teacher")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "teacher")]
        // GET: Moudels/Create

        public ActionResult CreateModoule()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateModoule([Bind(Include = "Id,Name,StartDate,EndDate,Description,Course")] Module module,int? courseId)
        {
            //var course = from dbCourse in db.Courses
            //    where dbCourse.Id == courseId
            //    select dbCourse;

            //var course = db.Courses.Find(courseId);


            if(courseId != null)
            module.CourseId = (int)courseId;
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.Courses.Find(courseId)?.CourseModules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Details",new{id = courseId});
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

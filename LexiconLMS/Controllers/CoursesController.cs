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
        public ActionResult Index(string filterString = null) {
            //if (!Request.IsAuthenticated) {
            //    return RedirectToAction("Login", "Account");
            //}

            var filteredCourses = db.Courses
                .Where(c => filterString == null || c.Name.Contains(filterString) ||
                            c.CourseStudents.FirstOrDefault(u => u.FirstName.Contains(filterString)).FirstName.Contains(filterString) ||
                            c.CourseStudents.FirstOrDefault(u => u.LastName.Contains(filterString)).LastName.Contains(filterString) ||
                            c.CourseModules.FirstOrDefault(m => m.Name.Contains(filterString)).Name.Contains(filterString) ||
                            c.Description.Contains(filterString)
                /* ||
                c.StartDate.ToString("yy-MM-dd").Contains(filterString) ||
                c.EndDate.ToString("yy-MM-dd").Contains(filterString)*/
                )
                .Select(c => new CourseVeiwModel {
                    Id = c.Id,
                    Name = c.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Description = c.Description,
                    CourseStudents = c.CourseStudents,
                    CourseModules = c.CourseModules
                });
            return View(filteredCourses);
        }

        // GET: Courses/Details/5
        [Authorize]
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null) {
                return HttpNotFound();
            }
            var students = db.Users.Where(u => u.CourseId == id);
            course.CourseStudents = students.ToList();
            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "teacher")]
        public ActionResult Create() {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,StartDate,EndDate,Description")] Course course) {
            if (ModelState.IsValid) {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }


        [Authorize(Roles = "teacher")]
        // GET: Courses/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null) {
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
        public ActionResult Edit([Bind(Include = "Id,Name,StartDate,EndDate,Description")] Course course) {
            if (ModelState.IsValid) {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "teacher")]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null) {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "teacher")]
        public ActionResult DeleteConfirmed(int id) {
            Course course = db.Courses.Find(id);
            db.Documents.RemoveRange(db.Documents.Where(d => d.CourseId == id));
            db.SaveChanges();
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Moudels/Create
        [Authorize(Roles = "teacher")]
        public ActionResult CreateModoule() {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateModoule([Bind(Include = "Id,Name,StartDate,EndDate,Description,Course")] Module module, int? courseId) {
            //var course = from dbCourse in db.Courses
            //    where dbCourse.Id == courseId
            //    select dbCourse;

            //var course = db.Courses.Find(courseId);
            if (courseId != null) {
                module.CourseId = (int)courseId;
            }
            var moduleCourse = db.Courses.FirstOrDefault(c => c.Id == module.CourseId);
            if (module.StartDate > moduleCourse.EndDate)
                ModelState.AddModelError("StartDate", "Module Start Date should be bafore " + moduleCourse.EndDate.Date.AddDays(1).ToShortDateString());
            if (module.StartDate < moduleCourse.StartDate)
                ModelState.AddModelError("StartDate", "Module Start Date should be After " + moduleCourse.StartDate.Date.AddDays(-1).ToShortDateString());
            if (module.EndDate > moduleCourse.EndDate)
                ModelState.AddModelError("EndDate", "Module End Date should be before " + moduleCourse.EndDate.Date.AddDays(1).ToShortDateString());
            if (module.EndDate < moduleCourse.StartDate)
                ModelState.AddModelError("EndDate", "Module End Date should be After " + moduleCourse.StartDate.Date.AddDays(-1).ToShortDateString());
            if (module.EndDate < module.StartDate)
                ModelState.AddModelError("", "Module End Date should be After Module Start Date");
            if (ModelState.IsValid) {
                db.Modules.Add(module);
                db.Courses.Find(courseId)?.CourseModules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
            //return RedirectToAction("Details", new { id = courseId });
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult StudendsOfCourse(int? id) {
            var course = db.Courses.Find(id);

            if (course != null) {
                var students = course.CourseStudents;
                return View(students);
            }

            return RedirectToAction("Details", id);
        }
    }
}

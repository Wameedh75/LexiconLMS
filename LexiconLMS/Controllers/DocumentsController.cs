using LexiconLMS.Models;
using LexiconLMS.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EntityState = System.Data.Entity.EntityState;

namespace LexiconLMS.Controllers
{
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private SelectList DocumentTypeSelectList() => new SelectList(db.DocumentTypes.ToList(), dataValueField: "Id", dataTextField: "Name");

        // GET: Documents
        public ActionResult Index() {
            return View(db.Documents.ToList());
        }

        // GET: Documents/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null) {
                return HttpNotFound();
            }
            return View(document);
        }

        public ActionResult Create(int? CourseId, int? ModuleId, int? ActivityId) {
            Activity activity = null;
            Module module = null;
            Course course = null;
            if (ActivityId != null) {
                activity = db.Activities.Find(ActivityId);
                module = activity?.Module;
                course = module?.Course;
            } else if (ModuleId != null) {
                module = db.Modules.Find(ModuleId);
                course = module?.Course;
            } else if (CourseId != null) {
                course = db.Courses.Find(CourseId);
            }
            var model = new DocumentViewModel {
                Types = DocumentTypeSelectList(),
                Course = course,
                Module = module,
                Activity = activity
            };
            return View(model);
        }

        //[Authorize(Roles = "teacher")]
        public ActionResult CreateForCourse(int? CourseId) {
            Course course = db.Courses.Find(CourseId);
            if (course == null) {
                return RedirectToAction("Index", "Courses");
            }
            var model = new DocumentViewModel { Types = DocumentTypeList.AsSelectList(), Course = course };
            return View(model);
        }

        public ActionResult CreateForModule(int? ModuleId) {
            Module module = db.Modules.Find(ModuleId);
            if (module == null) {
                return RedirectToAction("Index", "Courses");
            }
            var model = new DocumentViewModel { Types = DocumentTypeList.AsSelectList(), Module = module, Course = module.Course };
            return View(model);
        }

        public ActionResult CreateForActivity(int? ActivityId) {
            Activity activity = db.Activities.Find(ActivityId);
            if (activity == null) {
                return RedirectToAction("Index", "Courses");
            }
            var model = new DocumentViewModel { Types = DocumentTypeList.AsSelectList(), Activity = activity, Module = activity.Module, Course = activity.Module.Course };
            return View(model);
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null) {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Deadline")] Document document) {
            if (!ModelState.IsValid)
                return View(document);
            db.Entry(document).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null) {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();
            System.IO.File.Delete(document.FullPath);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

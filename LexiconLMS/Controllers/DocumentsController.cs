using LexiconLMS.Models;
using LexiconLMS.ViewModels;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace LexiconLMS.Controllers
{
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private SelectList DocumentTypeSelectList() => new SelectList(db.DocumentTypes.ToList(), dataValueField: "Id", dataTextField: "Name");

        private ActionResult RedirectToParent(Document document) {
            if (document.Activity != null) {
                return RedirectToAction("Details", "Activities", new { id = document.Activity.Id });
            } else if (document.Module != null) {
                return RedirectToAction("Details", "Modules", new { id = document.Module.Id });
            } else if (document.Course != null) {
                return RedirectToAction("Details", "Courses", new { id = document.Course.Id });
            } else {
                return RedirectToAction("Index", "Courses");
            }
        }

        private DocumentViewModel PopulateFromDocument(Document document) {
            return new DocumentViewModel {
                Document = document,
                SelectedTypeId = document.TypeId,
                FileName = document.FileName,
                CreatedBy = document?.User.FullName,
                Description = document.Description,
                Deadline = document.Deadline,
                CourseId = document.CourseId,
                ModuleId = document.ModuleId,
                ActivityId = document.ActivityId,
                Types = DocumentTypeSelectList(),
            };
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
                Activity = activity,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SelectedTypeId,File,Description,Deadline,Course.Id,Module.Id,Activity.Id")] DocumentViewModel model) {
            if (ModelState.IsValid) {
                string path = Path.Combine(Server.MapPath("~/Documents"), Path.GetFileName(model.File.FileName));
                model.File.SaveAs(path);
                var document = new Document {
                    TypeId = model.SelectedTypeId,
                    UserId = User.Identity.GetUserId(),
                    Description = model.Description,
                    Deadline = model.Deadline,
                    FileName = model.File.FileName,
                    MimeType = model.File.ContentType,
                    FullPath = path,
                    CourseId = model.CourseId,
                    ModuleId = model.ModuleId,
                    ActivityId = model.ActivityId,
                };
                db.Documents.Add(document);
                db.SaveChanges();
                return RedirectToParent(document);
            }
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

            DocumentViewModel model = PopulateFromDocument(document);
            return View(model);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SelectedTypeId,Description,Deadline")] DocumentViewModel model) {
            if (!ModelState.IsValid)
                return View(model);
            Document document = model.Document;
            document.TypeId = model.SelectedTypeId;
            document.Description = model.Description;
            document.Deadline = model.Deadline;
            db.SaveChanges();
            return RedirectToParent(document);
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
            DocumentViewModel model = PopulateFromDocument(document);
            return View(model);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();
            System.IO.File.Delete(document.FullPath);
            return RedirectToParent(document);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

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

        private SelectList DocumentTypeSelectList() {
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.SingleOrDefault(u => u.Id == userId);
            var userRoleIds = currentUser.Roles.Select(r => r.RoleId).ToList(); // List<string>
            var docTypes = db.DocumentTypes.Where(d => d.CanCreate.Count(c => userRoleIds.Contains(c.Id)) > 0).ToList();
            return new SelectList(docTypes, dataValueField: "Id", dataTextField: "Name");
        }

        private ActionResult RedirectToParent(Document document) {
            if (document.ActivityId != null) {
                return RedirectToAction("Details", "Activities", new { id = document.ActivityId });
            }
            if (document.ModuleId != null) {
                return RedirectToAction("Details", "Modules", new { id = document.ModuleId });
            }
            if (document.CourseId != null) {
                return RedirectToAction("Details", "Courses", new { id = document.CourseId });
            }
            return RedirectToAction("Index", "Courses");
        }

        private DocumentUpdateViewModel PopulateFromDocument(Document document) {
            return new DocumentUpdateViewModel {
                Document = document,
                SelectedTypeId = document.TypeId,
                FileName = document.FileName,
                Description = document.Description,
                Deadline = document.Deadline,
                CourseId = document.CourseId,
                CourseName = document.Course?.Name,
                ModuleId = document.ModuleId,
                ModuleName = document.Module?.Name,
                ActivityId = document.ActivityId,
                ActivityName = document.Activity?.Name,
                Types = DocumentTypeSelectList(),
            };
        }

        private DocumentUpdateViewModel FillNullValues(DocumentUpdateViewModel model) {
            model.CourseName = db.Courses.Find(model.CourseId)?.Name;
            model.ModuleName = db.Courses.Find(model.ModuleId)?.Name;
            model.ActivityName = db.Courses.Find(model.ActivityId)?.Name;
            model.Types = DocumentTypeSelectList();
            return model;
        }

        public ActionResult Create(int? CourseId, int? ModuleId, int? ActivityId) {
            if (CourseId == null && ModuleId == null && ActivityId == null) {
                return RedirectToAction("Index", "Courses");
            }
            var model = new DocumentInsertViewModel {
                Types = DocumentTypeSelectList(),
            };
            if (ActivityId != null) {
                Activity activity = db.Activities.Find(ActivityId);
                model.ActivityId = activity?.Id;
                model.ActivityName = activity?.Name;
            }
            if (ModuleId != null) {
                Module module = db.Modules.Find(ModuleId);
                model.ModuleId = module?.Id;
                model.ModuleName = module?.Name;
            }
            if (CourseId != null) {
                Course course = db.Courses.Find(CourseId);
                model.CourseId = course?.Id;
                model.CourseName = course?.Name;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SelectedTypeId,File,Description,Deadline,CourseId,ModuleId,ActivityId")] DocumentInsertViewModel model) {
            if (ModelState.IsValid) {
                string path = Server.MapPath("~/Documents");
                string relativePath = "";
                string filename = Path.GetFileName(model.File.FileName);
                if (model.CourseId != null) {
                    relativePath = Path.Combine("course", model.CourseId.ToString());
                } else if (model.ModuleId != null) {
                    relativePath = Path.Combine("module", model.ModuleId.ToString());
                } else if (model.ActivityId != null) {
                    relativePath = Path.Combine("activity", model.ActivityId.ToString());
                }
                path = Path.Combine(path, relativePath);
                Directory.CreateDirectory(path);
                string fullPath = Path.Combine(path, filename);
                int nr = 1;
                while (System.IO.File.Exists(fullPath)) {
                    filename = Path.GetFileNameWithoutExtension(filename) + "_" + nr + Path.GetExtension(filename);
                    fullPath = Path.Combine(path, filename);
                    nr++;
                }
                model.File.SaveAs(fullPath);
                var document = new Document {
                    TypeId = model.SelectedTypeId,
                    UserId = User.Identity.GetUserId(),
                    Description = model.Description,
                    Deadline = model.Deadline,
                    RelativePath = relativePath,
                    FileName = filename,
                    MimeType = model.File.ContentType,
                    FullPath = fullPath,
                    CourseId = model.CourseId,
                    ModuleId = model.ModuleId,
                    ActivityId = model.ActivityId
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
                return RedirectToAction("Index", "Courses");
            }
            Document document = db.Documents.Find(id);
            if (document == null) {
                return HttpNotFound();
            }
            DocumentUpdateViewModel model = PopulateFromDocument(document);
            return View(model);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SelectedTypeId,Description,Deadline,CourseId,ModuleId,ActivityId,Document")] DocumentUpdateViewModel model) {
            if (!ModelState.IsValid) {
                model = FillNullValues(model);
                return View(model);
            }
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
            DocumentUpdateViewModel model = PopulateFromDocument(document);
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

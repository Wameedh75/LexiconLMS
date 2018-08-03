using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LexiconLMS.Models
{
    public class DocumentType
    {
        public DocumentType() {
            CanCreate = new HashSet<IdentityRole>();
            CanView = new HashSet<IdentityRole>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Who can create")]
        public ICollection<IdentityRole> CanCreate { get; set; }

        [Display(Name = "Who can view")]
        public ICollection<IdentityRole> CanView { get; set; }
    }

    public static class DocumentTypeList
    {
        public static SelectList AsSelectList() {
            var db = ApplicationDbContext.Create();
            var documentTypeList = db.DocumentTypes.ToList();
            return new SelectList(documentTypeList, dataValueField: "Id", dataTextField: "Name");
        }
    }
}

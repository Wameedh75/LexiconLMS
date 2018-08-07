using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;

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
        public virtual ICollection<IdentityRole> CanCreate { get; set; }

        [Display(Name = "Who can view")]
        public virtual ICollection<IdentityRole> CanView { get; set; }

        public bool CanCreateByRoles(IEnumerable<string> roleIds) => roleIds != null && CanCreate.Count(c => roleIds.Contains(c.Id)) > 0;
        public bool CanCreateByUser(IPrincipal user) => CanCreateByRoles(_UserRoles(user));

        public bool CanViewByRoles(IEnumerable<string> roleIds) => roleIds != null && CanView.Count(c => roleIds.Contains(c.Id)) > 0;
        public bool CanViewByUser(IPrincipal user) => CanViewByRoles(_UserRoles(user));

        private IEnumerable<string> _UserRoles(IPrincipal user) {
            var db = new ApplicationDbContext();
            var userId = user.Identity.GetUserId();
            var applicationUser = db.Users.SingleOrDefault(u => u.Id == userId);
            return applicationUser?.Roles.Select(r => r.RoleId);
        }
    }
}

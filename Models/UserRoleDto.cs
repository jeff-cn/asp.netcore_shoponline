using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopOnline.Data;
using System.Collections.Generic;

namespace ShopOnline.Models {
    public class UserRoleDto {
        public UserRoleDto() {
        }

        public UserRoleDto(List<IdentityRole> roles, List<User> users) {
            Roles ??= new List<SelectListItem>(roles.Count);
            Users ??= new List<SelectListItem>(users.Count);
            foreach (var role in roles)
                Roles.Add(new SelectListItem(role.Name, role.Id));
            foreach (var user in users)
                Users.Add(new SelectListItem(user.Email, user.Id));
        }

        public string UserId { get; set; }

        public string RoleId { get; set; }

        public List<SelectListItem> Users { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }
}

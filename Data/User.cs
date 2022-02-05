using Microsoft.AspNetCore.Identity;
using System;

namespace ShopOnline.Data {
    public class User : IdentityUser {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Name { get; set; }

        public DateTime BirthDay { get; set; }
    }
}
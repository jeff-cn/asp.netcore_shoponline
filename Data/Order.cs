using System;
using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Data {
    public class Order {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Phone { get; set; }
        
        public DateTime DateTime { get; set; }
    }
}

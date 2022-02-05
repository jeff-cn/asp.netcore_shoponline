using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Data {
    public class ProductType {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Type")]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }
    }
}

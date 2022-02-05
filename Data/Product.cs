using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOnline.Data {
    public class Product {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product")]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Image { get; set; }

        public string Color { get; set; }

        public bool Available { get; set; }

        [Required]
        [Display(Name = "Product Type")]
        public int ProductTypeId { get; set; }

        [ForeignKey("ProductTypeId")]
        public ProductType ProductTypes { get; set; }

        [Required]
        [Display(Name = "Tag Name")]
        public int TagNameId { get; set; }

        [ForeignKey("TagNameId")]
        public TagName TagNames { get; set; }
    }
}

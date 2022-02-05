using System.ComponentModel.DataAnnotations;

namespace ShopOnline.Data {
    public class TagName {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tag Name")]
        [StringLength(15, MinimumLength = 4)]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Shporta24.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}

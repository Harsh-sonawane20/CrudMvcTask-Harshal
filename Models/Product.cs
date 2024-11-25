using System.ComponentModel.DataAnnotations;

namespace CrudMvcTask.Models
{
    public class Product
    {
        public int ProductId { get; set; }


        [Required(ErrorMessage = "Please fill this field")]
        public string ProductName { get; set; }
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "Please fill this field")]
        public string CategoryName { get; set; } 
    }
}

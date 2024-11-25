using System.ComponentModel.DataAnnotations;

namespace CrudMvcTask.Models
{
    public class Category
    {
        public int CategoryId { get; set; }



        [Required(ErrorMessage = "Please fill this field")]
        public string CategoryName { get; set; }
    }
}

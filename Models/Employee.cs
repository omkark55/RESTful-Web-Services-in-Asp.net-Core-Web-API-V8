using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace WebApi_Angular.Models
{
    public class Employee
    {
        [Key]
      public int  Id { get; set; }
     public string Name { get; set; }
        public string Gender { get; set; }
      public decimal  Salary { get; set; }
        public string Department { get; set; }
        public DateTime RecordUptoDate { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped] 
        public string ListType { get; set; }
       
      
    }
}

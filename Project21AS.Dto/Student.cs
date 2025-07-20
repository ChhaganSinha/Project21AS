using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project21AS.Dto
{
    public class Student : Base
    {
        public string Admin { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty; 
        [Required]
        public string Mobile { get; set; } = string.Empty;
        [Required]
        public string ? Batch { get; set; } 
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [NotMapped]
        public int MaxStudentPerBatch {  get; set; }


    }
}

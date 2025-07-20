using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project21AS.Dto
{
    public class StudentBatchTransfer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BatchAdmin { get; set; } = string.Empty;
        public string CurrentBatch { get; set; } = string.Empty;
        [Required(ErrorMessage = "This field is required")]
        public string ? NewBatch { get; set; }
    }
}

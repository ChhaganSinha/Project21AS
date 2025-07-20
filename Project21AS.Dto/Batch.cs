using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project21AS.Dto
{
    public class Batch : Base
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Admin { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public BatchStatus Status { get; set; }
        public int StudentCount { get; set; }

        [NotMapped]
        public int AllowedBatches { get; set; }
    }

    public enum BatchStatus
    {
        Enable = 0,
        Disable = 1
    }
}

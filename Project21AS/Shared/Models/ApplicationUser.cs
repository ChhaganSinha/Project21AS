
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project21AS.DataContext.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public int AllowedBatches { get; set; } = 4;
        public int MaxStudentPerBatch { get; set; } = 35;
        public bool IsActive { get; set; } = true;
    }
}

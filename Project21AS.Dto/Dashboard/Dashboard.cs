using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project21AS.Dto.Dashboard
{
    public class Dashboard
    {
        public int TotalBatches { get; set; }
        public int RemainingBatches { get; set; }
        public int TotalStudent { get; set; }
        public int TotalUsers { get; set; }
        public List<BatchStudentCount> StudentsByBatch { get; set; } = new();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project21AS.Dto;

namespace Project21AS.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options, bool ensureCreated = true) : base(options)
        {
            if (ensureCreated)
                Database.EnsureCreated();
        }

        public DbSet<Details> Details { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentFingerPrintMapping> StudentFingerPrintMapping { get; set; }
        public DbSet<Batch> Batch { get; set; }



    }
}

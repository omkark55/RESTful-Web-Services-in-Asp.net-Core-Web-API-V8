using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi_Angular.Models;


namespace WebApi_Angular.DBContext
{
    public class PracticeDbContext : DbContext
    {

        public PracticeDbContext(DbContextOptions<PracticeDbContext> options): base(options)
        {

        }

        public virtual DbSet<Employee> Employee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<Employee>().HasQueryFilter(z => !z.IsDeleted);
        }
      

    }
}

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

            modelBuilder.HasDefaultSchema("dbo");//map to default schema in SQL server
            //modelBuilder.RemovePluralizingTableNameConvention();
            modelBuilder.Entity<Employee>().HasQueryFilter(z => !z.IsDeleted);
           
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    optionsBuilder.UseSqlServer("PracticeContext");
        //    optionsBuilder.UseLazyLoadingProxies(false);
        //    base.OnConfiguring(optionsBuilder);
        //}

    }
}

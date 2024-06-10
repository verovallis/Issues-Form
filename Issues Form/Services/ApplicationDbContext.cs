using Issues_Form.Models;
using Microsoft.EntityFrameworkCore;

namespace Issues_Form.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Form> Form { get; set; }

        public DbSet<Category_Param> Category_Param { get; set; }

        // DbSet for Building_Param entity
        public DbSet<Building_Param> Building_Param { get; set; }

        // DbSet for Company_Param entity
        public DbSet<Company_Param> Company_Param { get; set; }

    }
}


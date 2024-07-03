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
        public DbSet<Building_Param> Building_Param { get; set; }
        public DbSet<Company_Param> Company_Param { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-OMH6Q4B\\MSSQLSERVER01;Initial Catalog=db_form;Integrated Security=True;Trust Server Certificate=True");
            }
        }
    }
}

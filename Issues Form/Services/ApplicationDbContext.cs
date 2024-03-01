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

    }
}

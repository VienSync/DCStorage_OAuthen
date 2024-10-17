using DCStorage.Models;
using System.Data.Entity;

namespace DCStorage.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    // Cấu hình các thuộc tính và mối quan hệ
        //    base.OnModelCreating(modelBuilder);
        //}

        public ApplicationDbContext() : base("ApplicationDbContext")
        {
        }

        public DbSet<C_ER001> C_ER001s { get; set; }
        public DbSet<C_ER002> C_ER002s { get; set; }
    }
}

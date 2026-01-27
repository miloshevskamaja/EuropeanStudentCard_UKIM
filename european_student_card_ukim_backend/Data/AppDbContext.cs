using EuropeanStudentCard.Models;
using Microsoft.EntityFrameworkCore;

namespace EuropeanStudentCard.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCard> StudentCards { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Card)
                .WithOne(c => c.Student)
                .HasForeignKey<StudentCard>(c => c.StudentId);
        }

    }
}

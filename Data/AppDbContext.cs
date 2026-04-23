using Microsoft.EntityFrameworkCore;
using IT15_LabExam_Tero.Models;

namespace IT15_LabExam_Tero.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .Property(e => e.DailyRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payroll>()
                .Property(p => p.DaysWorked)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Payroll>()
                .Property(p => p.Deduction)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payroll>()
                .Property(p => p.GrossPay)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payroll>()
                .Property(p => p.NetPay)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payroll>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Payrolls)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
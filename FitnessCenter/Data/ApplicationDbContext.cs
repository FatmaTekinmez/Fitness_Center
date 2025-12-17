using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Models;

namespace FitnessCenter.Data
{
    public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<GymCenter> GymCenters { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<TrainerService> TrainerServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //-------------------------------------------
            modelBuilder.Entity<Service>()
                .HasOne(s => s.FitnessCenter)
                .WithMany(g => g.Services)
                .HasForeignKey(s => s.FitnessCenterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.FitnessCenter)
                .WithMany(g => g.Trainers)
                .HasForeignKey(t => t.FitnessCenterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainerService>()
                .HasKey(ts => new { ts.TrainerId, ts.ServiceId });
            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerServices)
                .HasForeignKey(ts => ts.TrainerId);
            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Service)
                .WithMany(s => s.TrainerServices)
                .HasForeignKey(ts => ts.ServiceId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany()
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.ApplicationUserId);
        }
    }
}

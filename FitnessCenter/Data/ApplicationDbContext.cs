using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Models;

namespace FitnessCenter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<GymCenter> GymCenters { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Specialty> Specialities { get; set; }
        public DbSet<TrainerService> TrainerServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // SEQUENCES
            // =========================
            modelBuilder.HasSequence<int>("GymCenterSeq").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("ServiceSeq").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("TrainerSeq").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("TrainerAvailabilitySeq").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("SpecialtySeq").StartsAt(1).IncrementsBy(1);
            modelBuilder.HasSequence<int>("AppointmentSeq").StartsAt(1).IncrementsBy(1);

            // =========================
            // ID -> SEQUENCE BAĞLAMA
            // =========================
            modelBuilder.Entity<GymCenter>()
                .Property(g => g.Id)
                .HasDefaultValueSql("NEXT VALUE FOR GymCenterSeq");

            modelBuilder.Entity<Service>()
                .Property(s => s.Id)
                .HasDefaultValueSql("NEXT VALUE FOR ServiceSeq");

            modelBuilder.Entity<Trainer>()
                .Property(t => t.Id)
                .HasDefaultValueSql("NEXT VALUE FOR TrainerSeq");

            modelBuilder.Entity<TrainerAvailability>()
                .Property(t => t.Id)
                .HasDefaultValueSql("NEXT VALUE FOR TrainerAvailabilitySeq");

            modelBuilder.Entity<Specialty>()
                .Property(s => s.Id)
                .HasDefaultValueSql("NEXT VALUE FOR SpecialtySeq");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Id)
                .HasDefaultValueSql("NEXT VALUE FOR AppointmentSeq");

            // =========================
            // RELATIONSHIPS
            // =========================
            modelBuilder.Entity<Service>()
                .HasOne(s => s.GymCenter)
                .WithMany(g => g.Services)
                .HasForeignKey(s => s.FitnessCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.GymCenter)
                .WithMany(g => g.Trainers)
                .HasForeignKey(t => t.FitnessCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrainerService>()
                .HasKey(ts => new { ts.TrainerId, ts.ServiceId });

            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerServices)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.Service)
                .WithMany(s => s.TrainerServices)
                .HasForeignKey(ts => ts.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

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

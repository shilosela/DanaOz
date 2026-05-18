using Microsoft.EntityFrameworkCore;
using DanaOz.DAL.Entities;
using User = DanaOz.DAL.Entities.User;
using Subject = DanaOz.DAL.Entities.Subject;

namespace DanaOz.DAL.Context
{
    public class DanaOzDbContext : DbContext
    {
        public DanaOzDbContext(DbContextOptions<DanaOzDbContext> options) : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<UserSetting> User_Settings { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<UserClass> User_Classes { get; set; }
        public DbSet<UserTimetable> User_Timetable { get; set; }
        public DbSet<UserReminderSettings> User_ReminderSettings { get; set; }
        public DbSet<UserReminder> User_Reminders { get; set; }
        public DbSet<ChatLog> Chat_Logs { get; set; }
        public DbSet<UserPoints> User_Points { get; set; }
        public DbSet<UserMaterial> User_Materials { get; set; }
        public DbSet<Commercial> Commercials { get; set; }
        public DbSet<UserCommercialHistory> User_CommercialsHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map entity names to exact table names in Supabase
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<UserSetting>().ToTable("User_Settings");
            modelBuilder.Entity<School>().ToTable("Schools");
            modelBuilder.Entity<Subject>().ToTable("Subjects");
            modelBuilder.Entity<UserClass>().ToTable("User_Classes");
            modelBuilder.Entity<UserTimetable>().ToTable("User_Timetable");
            modelBuilder.Entity<UserReminderSettings>().ToTable("User_ReminderSettings");
            modelBuilder.Entity<UserReminder>().ToTable("User_Reminders");
            modelBuilder.Entity<ChatLog>().ToTable("Chat_Logs");
            modelBuilder.Entity<UserPoints>().ToTable("User_Points");
            modelBuilder.Entity<UserMaterial>().ToTable("User_Materials");
            modelBuilder.Entity<Commercial>().ToTable("Commercials");
            modelBuilder.Entity<UserCommercialHistory>().ToTable("User_CommercialsHistory");
        }
    }
}
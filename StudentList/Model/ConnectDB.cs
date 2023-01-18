using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentList.Model
{
    public class ConnectDB : DbContext
    {
        public DbSet<Student.Student> Students { get; set; }
        public DbSet<Group.StudyGroup> Groups { get; set; }
        public DbSet<Subject.StudySession> StudentSessions { get; set; }
        public DbSet<Subject.StudySubject> StudySubjects { get; set; }
        public DbSet<Student.StudentGrades> StudentGrades { get; set; }
        public DbSet<User> User { get; set; }
        public ConnectDB()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;user=3301;password=3301;database=studentlist;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(new User { Id=1, IsAdmin=true, Login="Admin", Password="Admin" }); 

        }
    }
}

using Microsoft.EntityFrameworkCore;
using FS02P2.Models;
using System.Configuration;

namespace FS02P2.Data
{
    public class SchoolContext:DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options ):base(options) { }

        public DbSet<Student> Students { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Department> Departments { get; set; }  
        public DbSet<Instructor> Instructors { get; set; }  
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }  
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        

        //we cant call tables in data base like this plural we need to call it with singular table names 
        //so we need to overrid it with the code below

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");
            

            //instructor id + course id = composit pk
            modelBuilder.Entity<CourseAssignment>().HasKey(c=> new {c.CourseID,c.InstructorID});
        }

    }
}

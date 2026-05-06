using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class LMMDbContext : IdentityDbContext<AppUser>
{
    public LMMDbContext(DbContextOptions<LMMDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRead> NotificationReads => Set<NotificationRead>();
    public DbSet<TeacherReview> TeacherReviews => Set<TeacherReview>();
    public DbSet<LearningMaterial> LearningMaterials => Set<LearningMaterial>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // AppUser
        builder.Entity<AppUser>(e =>
        {
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(15);
            e.Property(u => u.Address).HasMaxLength(200);
            e.Property(u => u.IsActive).HasDefaultValue(true);
            e.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Course
        builder.Entity<Course>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.Property(c => c.Description).HasMaxLength(2000);
            e.Property(c => c.Fee).HasColumnType("decimal(18,2)");
            e.Property(c => c.IsActive).HasDefaultValue(true);
            e.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Subject
        builder.Entity<Subject>(e =>
        {
            e.Property(s => s.Name).HasMaxLength(200).IsRequired();
            e.Property(s => s.Description).HasMaxLength(1000);
            e.HasOne(s => s.Course).WithMany(c => c.Subjects)
                .HasForeignKey(s => s.CourseId).OnDelete(DeleteBehavior.Cascade);
        });

        // Room
        builder.Entity<Room>(e =>
        {
            e.Property(r => r.Name).HasMaxLength(100).IsRequired();
            e.Property(r => r.Location).HasMaxLength(200);
            e.Property(r => r.IsActive).HasDefaultValue(true);
        });

        // Class
        builder.Entity<Class>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.HasOne(c => c.Course).WithMany(co => co.Classes)
                .HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.Teacher).WithMany()
                .HasForeignKey(c => c.TeacherId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.Room).WithMany(r => r.Classes)
                .HasForeignKey(c => c.RoomId).OnDelete(DeleteBehavior.Restrict);
        });

        // ClassSchedule
        builder.Entity<ClassSchedule>(e =>
        {
            e.HasOne(cs => cs.Class).WithMany(c => c.ClassSchedules)
                .HasForeignKey(cs => cs.ClassId).OnDelete(DeleteBehavior.Cascade);
        });

        // Enrollment
        builder.Entity<Enrollment>(e =>
        {
            e.Property(en => en.EnrollDate).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(en => en.Class).WithMany(c => c.Enrollments)
                .HasForeignKey(en => en.ClassId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(en => en.Student).WithMany()
                .HasForeignKey(en => en.StudentId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(en => new { en.StudentId, en.ClassId }).IsUnique();
        });

        // Attendance
        builder.Entity<Attendance>(e =>
        {
            e.Property(a => a.Note).HasMaxLength(500);
            e.HasOne(a => a.Class).WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Student).WithMany()
                .HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(a => new { a.ClassId, a.StudentId, a.Date }).IsUnique();
        });

        // Grade
        builder.Entity<Grade>(e =>
        {
            e.Property(g => g.Description).HasMaxLength(500);
            e.Property(g => g.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(g => g.Class).WithMany(c => c.Grades)
                .HasForeignKey(g => g.ClassId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(g => g.Student).WithMany()
                .HasForeignKey(g => g.StudentId).OnDelete(DeleteBehavior.Restrict);
        });

        // Payment
        builder.Entity<Payment>(e =>
        {
            e.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            e.Property(p => p.Note).HasMaxLength(500);
            e.Property(p => p.PaymentDate).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(p => p.Class).WithMany(c => c.Payments)
                .HasForeignKey(p => p.ClassId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Student).WithMany()
                .HasForeignKey(p => p.StudentId).OnDelete(DeleteBehavior.Restrict);
        });

        // Notification
        builder.Entity<Notification>(e =>
        {
            e.Property(n => n.Title).HasMaxLength(300).IsRequired();
            e.Property(n => n.Content).IsRequired();
            e.Property(n => n.TargetId).HasMaxLength(450);
            e.Property(n => n.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(n => n.Sender).WithMany()
                .HasForeignKey(n => n.SenderId).OnDelete(DeleteBehavior.Restrict);
        });

        // NotificationRead
        builder.Entity<NotificationRead>(e =>
        {
            e.Property(nr => nr.ReadAt).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(nr => nr.Notification).WithMany(n => n.NotificationReads)
                .HasForeignKey(nr => nr.NotificationId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(nr => nr.User).WithMany()
                .HasForeignKey(nr => nr.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(nr => new { nr.NotificationId, nr.UserId }).IsUnique();
        });

        // TeacherReview
        builder.Entity<TeacherReview>(e =>
        {
            e.Property(tr => tr.Comment).HasMaxLength(2000);
            e.Property(tr => tr.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(tr => tr.Student).WithMany()
                .HasForeignKey(tr => tr.StudentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(tr => tr.Teacher).WithMany()
                .HasForeignKey(tr => tr.TeacherId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(tr => new { tr.StudentId, tr.TeacherId, tr.ClassId }).IsUnique();
        });

        // LearningMaterial
        builder.Entity<LearningMaterial>(e =>
        {
            e.Property(m => m.Title).HasMaxLength(300).IsRequired();
            e.Property(m => m.Description).HasMaxLength(2000);
            e.Property(m => m.FilePath).HasMaxLength(500).IsRequired();
            e.Property(m => m.OriginalFileName).HasMaxLength(255).IsRequired();
            e.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(m => m.Class).WithMany(c => c.LearningMaterials)
                .HasForeignKey(m => m.ClassId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Teacher).WithMany()
                .HasForeignKey(m => m.TeacherId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}

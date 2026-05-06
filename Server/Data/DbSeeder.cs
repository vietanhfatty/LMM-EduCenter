using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LMMDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        await RepairLegacyIdentityMigrationHistoryAsync(context);
        await context.Database.MigrateAsync();
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedDemoStudentsAsync(userManager);
        await SeedDomainDataAsync(context, userManager);
    }

    private static async Task RepairLegacyIdentityMigrationHistoryAsync(LMMDbContext context)
    {
        var migrations = context.Database.GetMigrations().OrderBy(m => m).ToArray();
        if (migrations.Length == 0)
            return;

        const string historyTableName = "__EFMigrationsHistory";
        var identityTables = new[]
        {
            "AspNetRoles",
            "AspNetUsers",
            "AspNetRoleClaims",
            "AspNetUserClaims",
            "AspNetUserLogins",
            "AspNetUserRoles",
            "AspNetUserTokens"
        };
        var domainTables = new[]
        {
            "Courses",
            "Notifications",
            "Rooms",
            "TeacherReviews",
            "Subjects",
            "NotificationReads",
            "Classes",
            "Attendances",
            "ClassSchedules",
            "Enrollments",
            "Grades",
            "Payments"
        };

        var hasIdentitySchema = await AllTablesExistAsync(context, identityTables);
        var hasDomainSchema = await AllTablesExistAsync(context, domainTables);
        if (!hasIdentitySchema)
            return;

        var hasHistoryTable = await TableExistsAsync(context, historyTableName);
        if (!hasHistoryTable)
        {
            await context.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE [__EFMigrationsHistory] (
                    [MigrationId] nvarchar(150) NOT NULL,
                    [ProductVersion] nvarchar(32) NOT NULL,
                    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                );
                """);
        }

        var targetMigrations = hasDomainSchema ? migrations : new[] { migrations[0] };
        foreach (var migrationId in targetMigrations)
        {
            if (await HistoryRowExistsAsync(context, migrationId))
                continue;

            var efVersion = typeof(DbContext).Assembly.GetName().Version;
            var productVersion = efVersion is null
                ? "8.0.0"
                : $"{efVersion.Major}.{efVersion.Minor}.{efVersion.Build}";

            await context.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({migrationId}, {productVersion})");
        }
    }

    private static async Task<bool> AllTablesExistAsync(LMMDbContext context, IEnumerable<string> tableNames)
    {
        foreach (var tableName in tableNames)
        {
            if (!await TableExistsAsync(context, tableName))
                return false;
        }

        return true;
    }

    private static async Task<bool> TableExistsAsync(LMMDbContext context, string tableName)
    {
        var exists = await context.Database
            .SqlQueryRaw<int>("SELECT CASE WHEN OBJECT_ID({0}, 'U') IS NULL THEN 0 ELSE 1 END AS [Value]", tableName)
            .SingleAsync();

        return exists == 1;
    }

    private static async Task<bool> HistoryRowExistsAsync(LMMDbContext context, string migrationId)
    {
        var exists = await context.Database
            .SqlQueryRaw<int>("SELECT CASE WHEN EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {0}) THEN 1 ELSE 0 END AS [Value]", migrationId)
            .SingleAsync();

        return exists == 1;
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Staff", "Teacher", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedUsersAsync(UserManager<AppUser> userManager)
    {
        var seedUsers = new[]
        {
            new { Email = "admin@lmm.com", FullName = "Admin Staff", Password = "Admin@123", Role = "Staff" },
            new { Email = "teacher@lmm.com", FullName = "Nguyen Van Thay", Password = "Teacher@123", Role = "Teacher" },
            new { Email = "student@lmm.com", FullName = "Tran Van Hoc", Password = "Student@123", Role = "Student" }
        };

        foreach (var seed in seedUsers)
        {
            var user = await userManager.FindByEmailAsync(seed.Email);
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = seed.Email,
                    Email = seed.Email,
                    FullName = seed.FullName,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user, seed.Password);
                if (!createResult.Succeeded)
                    continue;
            }
            else
            {
                user.FullName = seed.FullName;
                user.UserName = seed.Email;
                user.Email = seed.Email;
                user.EmailConfirmed = true;
                user.IsActive = true;
                await userManager.UpdateAsync(user);

                var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                await userManager.ResetPasswordAsync(user, passwordResetToken, seed.Password);
            }

            if (!await userManager.IsInRoleAsync(user, seed.Role))
                await userManager.AddToRoleAsync(user, seed.Role);
        }
    }

    private static async Task SeedDemoStudentsAsync(UserManager<AppUser> userManager)
    {
        const string defaultPassword = "Student@123";

        for (var i = 1; i <= 30; i++)
        {
            var index = i.ToString("D2");
            var email = $"student{index}@lmm.com";
            var fullName = $"Demo Student {index}";
            var phone = $"0900000{index}";

            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    Phone = phone,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user, defaultPassword);
                if (!createResult.Succeeded)
                    continue;
            }
            else
            {
                user.UserName = email;
                user.Email = email;
                user.FullName = fullName;
                user.Phone = phone;
                user.EmailConfirmed = true;
                user.IsActive = true;
                await userManager.UpdateAsync(user);
            }

            if (!await userManager.IsInRoleAsync(user, "Student"))
                await userManager.AddToRoleAsync(user, "Student");
        }
    }

    private static async Task SeedDomainDataAsync(LMMDbContext context, UserManager<AppUser> userManager)
    {
        var admin = await userManager.FindByEmailAsync("admin@lmm.com");
        var teacher = await userManager.FindByEmailAsync("teacher@lmm.com");
        var student = await userManager.FindByEmailAsync("student@lmm.com");

        if (admin is null || teacher is null || student is null)
            return;

        if (!await context.Courses.AnyAsync())
        {
            context.Courses.AddRange(
                new Course
                {
                    Name = "English Foundation A1",
                    Description = "Beginner English communication and grammar.",
                    Fee = 3500000m,
                    DurationInHours = 48,
                    IsActive = true
                },
                new Course
                {
                    Name = "IELTS Intensive 5.5+",
                    Description = "Focused IELTS training for band 5.5 and above.",
                    Fee = 5200000m,
                    DurationInHours = 72,
                    IsActive = true
                });
            await context.SaveChangesAsync();
        }

        if (!await context.Subjects.AnyAsync())
        {
            var courses = await context.Courses.OrderBy(c => c.Id).ToListAsync();
            var foundationCourse = courses.First();
            var ieltsCourse = courses.Last();

            context.Subjects.AddRange(
                new Subject
                {
                    Name = "Grammar Basics",
                    Description = "Core grammar concepts for daily communication.",
                    CourseId = foundationCourse.Id
                },
                new Subject
                {
                    Name = "Speaking Practice",
                    Description = "Interactive speaking activities and pronunciation drills.",
                    CourseId = foundationCourse.Id
                },
                new Subject
                {
                    Name = "IELTS Writing Task 2",
                    Description = "Essay structure, coherence, and vocabulary improvement.",
                    CourseId = ieltsCourse.Id
                });
            await context.SaveChangesAsync();
        }

        if (!await context.Rooms.AnyAsync())
        {
            context.Rooms.AddRange(
                new Room { Name = "Room A101", Capacity = 25, Location = "Floor 1", IsActive = true },
                new Room { Name = "Room B203", Capacity = 20, Location = "Floor 2", IsActive = true });
            await context.SaveChangesAsync();
        }

        if (!await context.Classes.AnyAsync())
        {
            var foundationCourse = await context.Courses.OrderBy(c => c.Id).FirstAsync();
            var room = await context.Rooms.OrderBy(r => r.Id).FirstAsync();
            var startDate = DateTime.UtcNow.Date.AddDays(3);

            context.Classes.Add(new Class
            {
                Name = "ENG-A1-MORNING-01",
                CourseId = foundationCourse.Id,
                TeacherId = teacher.Id,
                RoomId = room.Id,
                StartDate = startDate,
                EndDate = startDate.AddDays(60),
                MaxStudents = 20,
                Status = (int)ClassStatus.Upcoming
            });
            await context.SaveChangesAsync();
        }

        var classEntity = await context.Classes.OrderBy(c => c.Id).FirstAsync();

        if (!await context.ClassSchedules.AnyAsync())
        {
            context.ClassSchedules.AddRange(
                new ClassSchedule
                {
                    ClassId = classEntity.Id,
                    DayOfWeek = (int)DayOfWeek.Monday,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(10, 0)
                },
                new ClassSchedule
                {
                    ClassId = classEntity.Id,
                    DayOfWeek = (int)DayOfWeek.Wednesday,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(10, 0)
                },
                new ClassSchedule
                {
                    ClassId = classEntity.Id,
                    DayOfWeek = (int)DayOfWeek.Friday,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(10, 0)
                });
            await context.SaveChangesAsync();
        }

        if (!await context.Enrollments.AnyAsync())
        {
            context.Enrollments.Add(new Enrollment
            {
                StudentId = student.Id,
                ClassId = classEntity.Id,
                Status = (int)EnrollmentStatus.Approved
            });
            await context.SaveChangesAsync();
        }

        if (!await context.Attendances.AnyAsync())
        {
            context.Attendances.AddRange(
                new Attendance
                {
                    ClassId = classEntity.Id,
                    StudentId = student.Id,
                    Date = DateTime.UtcNow.Date.AddDays(-1),
                    Status = (int)AttendanceStatus.Present,
                    Note = "On time"
                },
                new Attendance
                {
                    ClassId = classEntity.Id,
                    StudentId = student.Id,
                    Date = DateTime.UtcNow.Date,
                    Status = (int)AttendanceStatus.Late,
                    Note = "Arrived 10 minutes late"
                });
            await context.SaveChangesAsync();
        }

        if (!await context.Grades.AnyAsync())
        {
            context.Grades.AddRange(
                new Grade
                {
                    StudentId = student.Id,
                    ClassId = classEntity.Id,
                    Type = (int)GradeType.Quiz,
                    Description = "Vocabulary quiz week 1",
                    Score = 8.5,
                    Weight = 0.2
                },
                new Grade
                {
                    StudentId = student.Id,
                    ClassId = classEntity.Id,
                    Type = (int)GradeType.Assignment,
                    Description = "Writing assignment unit 1",
                    Score = 8.0,
                    Weight = 0.3
                });
            await context.SaveChangesAsync();
        }

        if (!await context.Payments.AnyAsync())
        {
            context.Payments.Add(new Payment
            {
                StudentId = student.Id,
                ClassId = classEntity.Id,
                Amount = 1750000m,
                Method = (int)PaymentMethod.BankTransfer,
                Status = (int)PaymentStatus.Completed,
                Note = "First installment"
            });
            await context.SaveChangesAsync();
        }

        if (!await context.Notifications.AnyAsync())
        {
            context.Notifications.Add(new Notification
            {
                Title = "Welcome to Learning Management System",
                Content = "Your class schedule has been published. Please check your dashboard.",
                SenderId = admin.Id,
                TargetType = (int)NotificationTargetType.All
            });
            await context.SaveChangesAsync();
        }

        var firstNotification = await context.Notifications.OrderBy(n => n.Id).FirstAsync();
        if (!await context.NotificationReads.AnyAsync())
        {
            context.NotificationReads.Add(new NotificationRead
            {
                NotificationId = firstNotification.Id,
                UserId = student.Id
            });
            await context.SaveChangesAsync();
        }

        if (!await context.TeacherReviews.AnyAsync())
        {
            context.TeacherReviews.Add(new TeacherReview
            {
                StudentId = student.Id,
                TeacherId = teacher.Id,
                ClassId = classEntity.Id,
                Rating = 5,
                Comment = "Teacher explains clearly and gives useful feedback."
            });
            await context.SaveChangesAsync();
        }
    }
}

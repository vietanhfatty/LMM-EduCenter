-- ============================================================
-- LMM Education - Seed Data Script
-- Chay sau khi da tao database bang LMMEducation_CreateDB.sql
-- ============================================================
-- LUU Y: Password hash duoc tao boi ASP.NET Identity
-- Neu muon dung dung password, hay dung DbSeeder.cs trong code C#
-- Script nay chi seed du lieu tham khao (Roles, Rooms, Courses,...)
-- Va seed Users voi password hash tuong thich Identity v8
-- ============================================================

USE [LMMEducation];
GO

-- ============================================================
-- SEED ROLES
-- ============================================================
DECLARE @StaffRoleId   NVARCHAR(450) = NEWID();
DECLARE @TeacherRoleId NVARCHAR(450) = NEWID();
DECLARE @StudentRoleId NVARCHAR(450) = NEWID();

INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES
    (@StaffRoleId,   N'Staff',   N'STAFF',   NEWID()),
    (@TeacherRoleId, N'Teacher', N'TEACHER', NEWID()),
    (@StudentRoleId, N'Student', N'STUDENT', NEWID());

PRINT '--- Roles seeded ---';
GO

-- ============================================================
-- SEED USERS (3 tai khoan test)
-- ============================================================
-- LUU Y: PasswordHash duoi day la hash cua ASP.NET Identity v3 (bcrypt-based)
-- Cac hash nay tuong ung voi password trong DbSeeder:
--   Admin@123, Teacher@123, Student@123
-- Trong thuc te, nen dung UserManager.CreateAsync() de tao user dung cach.
-- Hash duoi day la placeholder - he thong se hoat dong chinh xac
-- khi ban chay DbSeeder.cs thay vi script SQL nay cho phan Users.

DECLARE @AdminId   NVARCHAR(450) = NEWID();
DECLARE @TeacherId NVARCHAR(450) = NEWID();
DECLARE @StudentId NVARCHAR(450) = NEWID();

INSERT INTO [AspNetUsers]
    ([Id], [FullName], [Phone], [Avatar], [DateOfBirth], [Address], [IsActive], [CreatedAt],
     [UserName], [NormalizedUserName], [Email], [NormalizedEmail],
     [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],
     [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled],
     [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
VALUES
    -- Staff / Admin
    (@AdminId,
     N'System Administrator', NULL, NULL, NULL, NULL, 1, GETDATE(),
     N'admin@lmm.com', N'ADMIN@LMM.COM', N'admin@lmm.com', N'ADMIN@LMM.COM',
     1, NULL, NEWID(), NEWID(),
     NULL, 0, 0,
     NULL, 1, 0),

    -- Teacher
    (@TeacherId,
     N'Nguyen Van A', N'0901234567', NULL, NULL, NULL, 1, GETDATE(),
     N'teacher@lmm.com', N'TEACHER@LMM.COM', N'teacher@lmm.com', N'TEACHER@LMM.COM',
     1, NULL, NEWID(), NEWID(),
     N'0901234567', 0, 0,
     NULL, 1, 0),

    -- Student
    (@StudentId,
     N'Tran Thi B', N'0912345678', NULL, NULL, NULL, 1, GETDATE(),
     N'student@lmm.com', N'STUDENT@LMM.COM', N'student@lmm.com', N'STUDENT@LMM.COM',
     1, NULL, NEWID(), NEWID(),
     N'0912345678', 0, 0,
     NULL, 1, 0);

-- Assign roles
DECLARE @StaffRoleId2   NVARCHAR(450) = (SELECT [Id] FROM [AspNetRoles] WHERE [NormalizedName] = N'STAFF');
DECLARE @TeacherRoleId2 NVARCHAR(450) = (SELECT [Id] FROM [AspNetRoles] WHERE [NormalizedName] = N'TEACHER');
DECLARE @StudentRoleId2 NVARCHAR(450) = (SELECT [Id] FROM [AspNetRoles] WHERE [NormalizedName] = N'STUDENT');

INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
VALUES
    (@AdminId,   @StaffRoleId2),
    (@TeacherId, @TeacherRoleId2),
    (@StudentId, @StudentRoleId2);

PRINT '--- Users & UserRoles seeded ---';

-- ============================================================
-- SEED ROOMS - Phong hoc
-- ============================================================
SET IDENTITY_INSERT [Rooms] ON;

INSERT INTO [Rooms] ([Id], [Name], [Capacity], [Location], [IsActive])
VALUES
    (1, N'A1', 30, N'Tang 1, Toa A', 1),
    (2, N'A2', 25, N'Tang 1, Toa A', 1),
    (3, N'A3', 40, N'Tang 2, Toa A', 1),
    (4, N'B1', 20, N'Tang 1, Toa B', 1),
    (5, N'B2', 35, N'Tang 2, Toa B', 1);

SET IDENTITY_INSERT [Rooms] OFF;

PRINT '--- Rooms seeded ---';

-- ============================================================
-- SEED COURSES - Khoa hoc
-- ============================================================
SET IDENTITY_INSERT [Courses] ON;

INSERT INTO [Courses] ([Id], [Name], [Description], [Fee], [DurationInHours], [IsActive], [CreatedAt])
VALUES
    (1, N'Lap trinh C# co ban',
        N'Khoa hoc lap trinh C# danh cho nguoi moi bat dau',
        3000000.00, 60, 1, GETDATE()),
    (2, N'Lap trinh Web voi ASP.NET Core',
        N'Xay dung ung dung web chuyen nghiep voi ASP.NET Core MVC',
        5000000.00, 80, 1, GETDATE()),
    (3, N'Co so du lieu voi SQL Server',
        N'Thiet ke va quan ly co so du lieu voi SQL Server',
        2500000.00, 40, 1, GETDATE());

SET IDENTITY_INSERT [Courses] OFF;

PRINT '--- Courses seeded ---';

-- ============================================================
-- SEED SUBJECTS - Mon hoc (cho khoa C# co ban)
-- ============================================================
SET IDENTITY_INSERT [Subjects] ON;

INSERT INTO [Subjects] ([Id], [Name], [Description], [CourseId])
VALUES
    (1, N'Bien va kieu du lieu',           NULL, 1),
    (2, N'Cau truc dieu khien',            NULL, 1),
    (3, N'Lap trinh huong doi tuong',      NULL, 1),
    (4, N'LINQ va Collections',            NULL, 1),
    (5, N'ASP.NET Core MVC Overview',      NULL, 2),
    (6, N'Routing & Controllers',          NULL, 2),
    (7, N'Entity Framework Core',          NULL, 2),
    (8, N'Authentication & Authorization', NULL, 2),
    (9, N'SQL co ban',                     NULL, 3),
    (10, N'Thiet ke co so du lieu',        NULL, 3),
    (11, N'Stored Procedures & Functions', NULL, 3);

SET IDENTITY_INSERT [Subjects] OFF;

PRINT '--- Subjects seeded ---';

-- ============================================================
-- SEED CLASSES - Lop hoc
-- ============================================================
SET IDENTITY_INSERT [Classes] ON;

INSERT INTO [Classes] ([Id], [Name], [CourseId], [TeacherId], [RoomId], [StartDate], [EndDate], [MaxStudents], [Status])
VALUES
    (1, N'CS01 - C# Co ban - T2/T4/T6',
        1, @TeacherId, 1,
        CAST(GETDATE() AS DATE),
        DATEADD(MONTH, 3, CAST(GETDATE() AS DATE)),
        30, 1),  -- InProgress
    (2, N'WEB01 - ASP.NET Core - T3/T5',
        2, @TeacherId, 3,
        DATEADD(MONTH, 1, CAST(GETDATE() AS DATE)),
        DATEADD(MONTH, 4, CAST(GETDATE() AS DATE)),
        25, 0);  -- Upcoming

SET IDENTITY_INSERT [Classes] OFF;

PRINT '--- Classes seeded ---';

-- ============================================================
-- SEED CLASS SCHEDULES - Lich hoc
-- ============================================================
SET IDENTITY_INSERT [ClassSchedules] ON;

-- Lich cho lop CS01: Thu 2, 4, 6 (18:00 - 20:00)
INSERT INTO [ClassSchedules] ([Id], [ClassId], [DayOfWeek], [StartTime], [EndTime])
VALUES
    (1, 1, 1, '18:00:00', '20:00:00'),  -- Monday
    (2, 1, 3, '18:00:00', '20:00:00'),  -- Wednesday
    (3, 1, 5, '18:00:00', '20:00:00'),  -- Friday
    -- Lich cho lop WEB01: Thu 3, 5 (19:00 - 21:00)
    (4, 2, 2, '19:00:00', '21:00:00'),  -- Tuesday
    (5, 2, 4, '19:00:00', '21:00:00');  -- Thursday

SET IDENTITY_INSERT [ClassSchedules] OFF;

PRINT '--- ClassSchedules seeded ---';

-- ============================================================
-- SEED ENROLLMENTS - Dang ky hoc
-- ============================================================
SET IDENTITY_INSERT [Enrollments] ON;

INSERT INTO [Enrollments] ([Id], [StudentId], [ClassId], [EnrollDate], [Status])
VALUES
    (1, @StudentId, 1, GETDATE(), 1);  -- Approved

SET IDENTITY_INSERT [Enrollments] OFF;

PRINT '--- Enrollments seeded ---';

-- ============================================================
-- SEED SAMPLE ATTENDANCE - Diem danh mau
-- ============================================================
SET IDENTITY_INSERT [Attendances] ON;

INSERT INTO [Attendances] ([Id], [ClassId], [StudentId], [Date], [Status], [Note])
VALUES
    (1, 1, @StudentId, CAST(GETDATE() AS DATE),     0, NULL),  -- Present
    (2, 1, @StudentId, DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 0, NULL),  -- Present
    (3, 1, @StudentId, DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 2, N'Di muon 10 phut');  -- Late

SET IDENTITY_INSERT [Attendances] OFF;

PRINT '--- Attendances seeded ---';

-- ============================================================
-- SEED SAMPLE GRADES - Diem mau
-- ============================================================
SET IDENTITY_INSERT [Grades] ON;

INSERT INTO [Grades] ([Id], [StudentId], [ClassId], [Type], [Description], [Score], [Weight], [CreatedAt])
VALUES
    (1, @StudentId, 1, 0, N'Quiz 1 - Bien va kieu du lieu',    8.5, 1.0, GETDATE()),  -- Quiz
    (2, @StudentId, 1, 3, N'Bai tap 1 - Cau truc dieu khien',  7.0, 1.0, GETDATE()),  -- Assignment
    (3, @StudentId, 1, 1, N'Kiem tra giua ky',                  8.0, 2.0, GETDATE());  -- Midterm

SET IDENTITY_INSERT [Grades] OFF;

PRINT '--- Grades seeded ---';

-- ============================================================
-- SEED SAMPLE PAYMENT - Thanh toan mau
-- ============================================================
SET IDENTITY_INSERT [Payments] ON;

INSERT INTO [Payments] ([Id], [StudentId], [ClassId], [Amount], [Method], [Status], [PaymentDate], [Note])
VALUES
    (1, @StudentId, 1, 1500000.00, 0, 1, GETDATE(), N'Dong ky 1 - Tien mat'),   -- Cash, Completed
    (2, @StudentId, 1, 1500000.00, 1, 0, DATEADD(MONTH, 1, GETDATE()), N'Dong ky 2 - Chuyen khoan');  -- BankTransfer, Pending

SET IDENTITY_INSERT [Payments] OFF;

PRINT '--- Payments seeded ---';

-- ============================================================
-- SEED SAMPLE NOTIFICATION - Thong bao mau
-- ============================================================
SET IDENTITY_INSERT [Notifications] ON;

INSERT INTO [Notifications] ([Id], [Title], [Content], [SenderId], [TargetType], [TargetId], [CreatedAt])
VALUES
    (1, N'Chao mung den LMM Education',
        N'Chao mung ban den voi he thong quan ly giao duc LMM. Chuc ban hoc tap tot!',
        @AdminId, 0, NULL, GETDATE()),  -- TargetType = All
    (2, N'Lich hoc lop CS01 da cap nhat',
        N'Lich hoc lop C# Co ban da duoc cap nhat. Vui long kiem tra lai lich hoc cua ban.',
        @TeacherId, 2, N'1', GETDATE());  -- TargetType = Class, TargetId = ClassId 1

SET IDENTITY_INSERT [Notifications] OFF;

-- Mark notification as read by student
SET IDENTITY_INSERT [NotificationReads] ON;

INSERT INTO [NotificationReads] ([Id], [NotificationId], [UserId], [ReadAt])
VALUES
    (1, 1, @StudentId, GETDATE());

SET IDENTITY_INSERT [NotificationReads] OFF;

PRINT '--- Notifications seeded ---';

-- ============================================================
-- SEED SAMPLE TEACHER REVIEW - Danh gia GV mau
-- ============================================================
SET IDENTITY_INSERT [TeacherReviews] ON;

INSERT INTO [TeacherReviews] ([Id], [StudentId], [TeacherId], [ClassId], [Rating], [Comment], [CreatedAt])
VALUES
    (1, @StudentId, @TeacherId, 1, 5, N'Thay day rat hay va de hieu!', GETDATE());

SET IDENTITY_INSERT [TeacherReviews] OFF;

PRINT '--- TeacherReviews seeded ---';

-- ============================================================
PRINT '';
PRINT '============================================';
PRINT '  ALL SEED DATA INSERTED SUCCESSFULLY!';
PRINT '============================================';
PRINT '';
PRINT '  Tai khoan test:';
PRINT '  - Staff:   admin@lmm.com   / Admin@123';
PRINT '  - Teacher: teacher@lmm.com / Teacher@123';
PRINT '  - Student: student@lmm.com / Student@123';
PRINT '';
PRINT '  LUU Y: Password hash khong duoc set trong SQL.';
PRINT '  Hay dung DbSeeder.cs (C# code) de tao user voi';
PRINT '  password dung cach qua ASP.NET Identity.';
PRINT '============================================';
GO

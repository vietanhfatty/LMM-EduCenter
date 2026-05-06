-- ============================================================
-- LMM Education - Demo: 5 lớp ĐANG DIỄN RA (Status = 1)
-- Chạy sau khi đã có: Courses, Rooms, giáo viên teacher01..07,
-- học viên student01..50 (ví dụ LMMEducation_Seed_BulkUsersCoursesClasses.sql)
-- ============================================================

USE [LMMEducation];
GO

SET NOCOUNT ON;

PRINT '=== START SEED DEMO IN-PROGRESS CLASSES ===';

DECLARE @StudentRoleId NVARCHAR(450) = (
    SELECT TOP 1 [Id] FROM [AspNetRoles] WHERE [NormalizedName] = N'STUDENT'
);

IF @StudentRoleId IS NULL
BEGIN
    THROW 50002, 'Missing STUDENT role. Seed roles first.', 1;
END;

DECLARE @TeacherRoleId NVARCHAR(450) = (
    SELECT TOP 1 [Id] FROM [AspNetRoles] WHERE [NormalizedName] = N'TEACHER'
);

IF @TeacherRoleId IS NULL
BEGIN
    THROW 50004, 'Missing TEACHER role. Seed roles first.', 1;
END;

-- Ensure required courses exist
INSERT INTO [Courses] ([Name], [Description], [Fee], [DurationInHours], [IsActive], [CreatedAt])
SELECT x.[Name], x.[Description], x.[Fee], x.[DurationInHours], 1, GETDATE()
FROM (VALUES
    (N'English Foundation A1', N'Beginner English communication and grammar.', CAST(3500000.00 AS DECIMAL(18,2)), 48),
    (N'IELTS Intensive 5.5+', N'Focused IELTS training for band 5.5 and above.', CAST(5200000.00 AS DECIMAL(18,2)), 72),
    (N'TOEIC Bridge 450+', N'Build core TOEIC listening and reading skills.', CAST(4200000.00 AS DECIMAL(18,2)), 60),
    (N'English Communication B1', N'Develop practical communication in workplace settings.', CAST(3900000.00 AS DECIMAL(18,2)), 54),
    (N'Business English Essentials', N'Email, meeting, and presentation skills in English.', CAST(4800000.00 AS DECIMAL(18,2)), 66)
) x([Name], [Description], [Fee], [DurationInHours])
WHERE NOT EXISTS (SELECT 1 FROM [Courses] c WHERE c.[Name] = x.[Name]);

-- Ensure required rooms exist
INSERT INTO [Rooms] ([Name], [Capacity], [Location], [IsActive])
SELECT x.[Name], x.[Capacity], x.[Location], 1
FROM (VALUES
    (N'Room A101', 30, N'Floor 1'),
    (N'Room B203', 25, N'Floor 2'),
    (N'Room C301', 30, N'Floor 3'),
    (N'Room C302', 28, N'Floor 3'),
    (N'Room D401', 35, N'Floor 4')
) x([Name], [Capacity], [Location])
WHERE NOT EXISTS (SELECT 1 FROM [Rooms] r WHERE r.[Name] = x.[Name]);

DECLARE @TeacherPool TABLE (Rn INT NOT NULL, TeacherId NVARCHAR(450) NOT NULL);
INSERT INTO @TeacherPool (Rn, TeacherId)
SELECT
    ROW_NUMBER() OVER (ORDER BY u.[Email]) AS Rn,
    u.[Id]
FROM [AspNetUsers] u
INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id] AND ur.[RoleId] = @TeacherRoleId
WHERE u.[IsActive] = 1;

IF (SELECT COUNT(1) FROM @TeacherPool) = 0
BEGIN
    THROW 50005, 'Không có giáo viên nào trong hệ thống để gán lớp demo.', 1;
END;

DECLARE @T1 NVARCHAR(450) = (SELECT TOP 1 TeacherId FROM @TeacherPool WHERE Rn = 1);
DECLARE @T2 NVARCHAR(450) = (SELECT TOP 1 TeacherId FROM @TeacherPool WHERE Rn = 2);
DECLARE @T3 NVARCHAR(450) = (SELECT TOP 1 TeacherId FROM @TeacherPool WHERE Rn = 3);
DECLARE @T4 NVARCHAR(450) = (SELECT TOP 1 TeacherId FROM @TeacherPool WHERE Rn = 4);
DECLARE @T5 NVARCHAR(450) = (SELECT TOP 1 TeacherId FROM @TeacherPool WHERE Rn = 5);
DECLARE @TeacherFallback NVARCHAR(450) = (SELECT TOP 1 TeacherId FROM @TeacherPool ORDER BY Rn);

SET @T1 = ISNULL(@T1, @TeacherFallback);
SET @T2 = ISNULL(@T2, @TeacherFallback);
SET @T3 = ISNULL(@T3, @TeacherFallback);
SET @T4 = ISNULL(@T4, @TeacherFallback);
SET @T5 = ISNULL(@T5, @TeacherFallback);

DECLARE @R1 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room A101');
DECLARE @R2 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room B203');
DECLARE @R3 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room C301');
DECLARE @R4 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room C302');
DECLARE @R5 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room D401');

DECLARE @C1 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'English Foundation A1');
DECLARE @C2 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'IELTS Intensive 5.5+');
DECLARE @C3 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'TOEIC Bridge 450+');
DECLARE @C4 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'English Communication B1');
DECLARE @C5 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'Business English Essentials');

-- Đang diễn ra: đã bắt đầu, chưa kết thúc (theo ngày)
DECLARE @StartPast DATE = CAST(DATEADD(DAY, -21, GETDATE()) AS DATE);
DECLARE @EndFuture DATE = CAST(DATEADD(DAY, 90, GETDATE()) AS DATE);

INSERT INTO [Classes] ([Name], [CourseId], [TeacherId], [RoomId], [StartDate], [EndDate], [MaxStudents], [Status])
SELECT x.[Name], x.[CourseId], x.[TeacherId], x.[RoomId], x.[StartDate], x.[EndDate], 30, 1
FROM (VALUES
    (N'DEMO-IP-A1',        @C1, @T1, @R1, @StartPast, @EndFuture),
    (N'DEMO-IP-IELTS',     @C2, @T2, @R2, @StartPast, @EndFuture),
    (N'DEMO-IP-TOEIC',     @C3, @T3, @R3, @StartPast, @EndFuture),
    (N'DEMO-IP-B1',        @C4, @T4, @R4, @StartPast, @EndFuture),
    (N'DEMO-IP-BUSINESS',  @C5, @T5, @R5, @StartPast, @EndFuture)
) x([Name], [CourseId], [TeacherId], [RoomId], [StartDate], [EndDate])
WHERE NOT EXISTS (SELECT 1 FROM [Classes] c WHERE c.[Name] = x.[Name]);

PRINT '--- Inserted demo in-progress classes (if missing) ---';

INSERT INTO [ClassSchedules] ([ClassId], [DayOfWeek], [StartTime], [EndTime])
SELECT c.[Id], s.[DayOfWeek], s.[StartTime], s.[EndTime]
FROM [Classes] c
JOIN (VALUES
    (N'DEMO-IP-A1',       1, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'DEMO-IP-A1',       3, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'DEMO-IP-IELTS',    2, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME)),
    (N'DEMO-IP-IELTS',    4, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME)),
    (N'DEMO-IP-TOEIC',    1, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
    (N'DEMO-IP-TOEIC',    5, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
    (N'DEMO-IP-B1',       3, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'DEMO-IP-B1',       6, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'DEMO-IP-BUSINESS', 2, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
    (N'DEMO-IP-BUSINESS', 6, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME))
) s([ClassName], [DayOfWeek], [StartTime], [EndTime]) ON c.[Name] = s.[ClassName]
WHERE NOT EXISTS (
    SELECT 1 FROM [ClassSchedules] cs
    WHERE cs.[ClassId] = c.[Id]
      AND cs.[DayOfWeek] = s.[DayOfWeek]
      AND cs.[StartTime] = s.[StartTime]
      AND cs.[EndTime] = s.[EndTime]
);

PRINT '--- Seeded class schedules for DEMO-IP-* ---';

-- Gán ~10 học viên / lớp (student01..10 -> A1, 11..20 -> IELTS, ...)
DECLARE @DemoClasses TABLE (ClassName NVARCHAR(100) NOT NULL, Lo INT NOT NULL, Hi INT NOT NULL);

INSERT INTO @DemoClasses (ClassName, Lo, Hi)
VALUES
    (N'DEMO-IP-A1',        1, 10),
    (N'DEMO-IP-IELTS',     11, 20),
    (N'DEMO-IP-TOEIC',     21, 30),
    (N'DEMO-IP-B1',        31, 40),
    (N'DEMO-IP-BUSINESS',  41, 50);

;WITH StudentPool AS
(
    SELECT
        u.[Id] AS StudentId,
        ROW_NUMBER() OVER (ORDER BY u.[Email]) AS Rn
    FROM [AspNetUsers] u
    INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id] AND ur.[RoleId] = @StudentRoleId
    WHERE u.[IsActive] = 1
)
INSERT INTO [Enrollments] ([StudentId], [ClassId], [EnrollDate], [Status])
SELECT sp.StudentId, c.[Id], GETDATE(), 1
FROM @DemoClasses dc
INNER JOIN [Classes] c ON c.[Name] = dc.ClassName
INNER JOIN StudentPool sp ON sp.Rn BETWEEN dc.Lo AND dc.Hi
WHERE NOT EXISTS (
    SELECT 1 FROM [Enrollments] e
    WHERE e.[StudentId] = sp.StudentId AND e.[ClassId] = c.[Id]
);

PRINT '--- Enrolled students (student01..50) into 5 demo classes ---';
PRINT '=== DEMO IN-PROGRESS SEED COMPLETED ===';
GO

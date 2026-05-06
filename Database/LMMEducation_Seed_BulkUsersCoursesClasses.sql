USE [LMMEducation];
GO

SET NOCOUNT ON;

PRINT '=== START SEED BULK DATA ===';

DECLARE @TeacherRoleId NVARCHAR(450) = (
    SELECT TOP 1 [Id] FROM [AspNetRoles] WHERE [NormalizedName] = N'TEACHER'
);
DECLARE @StudentRoleId NVARCHAR(450) = (
    SELECT TOP 1 [Id] FROM [AspNetRoles] WHERE [NormalizedName] = N'STUDENT'
);

IF @TeacherRoleId IS NULL OR @StudentRoleId IS NULL
BEGIN
    THROW 50001, 'Missing roles TEACHER/STUDENT. Seed roles first.', 1;
END;

DECLARE @Teachers TABLE (
    Email NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NULL
);

INSERT INTO @Teachers (Email, FullName, Phone)
VALUES
    (N'teacher01@lmm.com', N'Nguyen Minh Anh', N'091100001'),
    (N'teacher02@lmm.com', N'Tran Quoc Bao', N'091100002'),
    (N'teacher03@lmm.com', N'Le Thu Ha', N'091100003'),
    (N'teacher04@lmm.com', N'Pham Gia Huy', N'091100004'),
    (N'teacher05@lmm.com', N'Vo Thanh Long', N'091100005'),
    (N'teacher06@lmm.com', N'Bui Khanh Linh', N'091100006'),
    (N'teacher07@lmm.com', N'Doan Tuan Kiet', N'091100007');

INSERT INTO [AspNetUsers]
(
    [Id], [FullName], [Phone], [Avatar], [DateOfBirth], [Address], [IsActive], [CreatedAt],
    [UserName], [NormalizedUserName], [Email], [NormalizedEmail],
    [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],
    [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd],
    [LockoutEnabled], [AccessFailedCount]
)
SELECT
    NEWID(),
    t.FullName,
    t.Phone,
    NULL, NULL, NULL, 1, GETDATE(),
    t.Email, UPPER(t.Email), t.Email, UPPER(t.Email),
    1, NULL, NEWID(), NEWID(),
    t.Phone, 0, 0, NULL, 1, 0
FROM @Teachers t
WHERE NOT EXISTS (
    SELECT 1 FROM [AspNetUsers] u WHERE u.[NormalizedEmail] = UPPER(t.Email)
);

INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
SELECT u.[Id], @TeacherRoleId
FROM [AspNetUsers] u
INNER JOIN @Teachers t ON UPPER(t.Email) = u.[NormalizedEmail]
WHERE NOT EXISTS (
    SELECT 1 FROM [AspNetUserRoles] ur
    WHERE ur.[UserId] = u.[Id] AND ur.[RoleId] = @TeacherRoleId
);

PRINT '--- Seeded 7 teachers ---';

DECLARE @Students TABLE (
    Email NVARCHAR(256) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NULL
);

INSERT INTO @Students (Email, FullName, Phone)
VALUES
    (N'student01@lmm.com', N'Nguyen Hoang Anh', N'090900001'),
    (N'student02@lmm.com', N'Tran Minh Chau', N'090900002'),
    (N'student03@lmm.com', N'Le Quoc Dat', N'090900003'),
    (N'student04@lmm.com', N'Pham Thu Dung', N'090900004'),
    (N'student05@lmm.com', N'Vo Gia Huy', N'090900005'),
    (N'student06@lmm.com', N'Bui Anh Khoa', N'090900006'),
    (N'student07@lmm.com', N'Dang Ngoc Lam', N'090900007'),
    (N'student08@lmm.com', N'Hoang Thao Linh', N'090900008'),
    (N'student09@lmm.com', N'Do Minh Long', N'090900009'),
    (N'student10@lmm.com', N'Pham Bao Ngan', N'090900010'),
    (N'student11@lmm.com', N'Nguyen Quoc Nam', N'090900011'),
    (N'student12@lmm.com', N'Tran Khai Nguyen', N'090900012'),
    (N'student13@lmm.com', N'Le Thu Phuong', N'090900013'),
    (N'student14@lmm.com', N'Vo Duc Quan', N'090900014'),
    (N'student15@lmm.com', N'Bui Gia Han', N'090900015'),
    (N'student16@lmm.com', N'Dang Minh Hieu', N'090900016'),
    (N'student17@lmm.com', N'Hoang Lan Huong', N'090900017'),
    (N'student18@lmm.com', N'Do Tuan Kiet', N'090900018'),
    (N'student19@lmm.com', N'Pham Gia Linh', N'090900019'),
    (N'student20@lmm.com', N'Nguyen Hoai My', N'090900020'),
    (N'student21@lmm.com', N'Tran Bao Nhi', N'090900021'),
    (N'student22@lmm.com', N'Le Minh Phuc', N'090900022'),
    (N'student23@lmm.com', N'Vo Nhat Quang', N'090900023'),
    (N'student24@lmm.com', N'Bui Thanh Son', N'090900024'),
    (N'student25@lmm.com', N'Dang Minh Tam', N'090900025'),
    (N'student26@lmm.com', N'Hoang Gia Uyen', N'090900026'),
    (N'student27@lmm.com', N'Do Khanh Van', N'090900027'),
    (N'student28@lmm.com', N'Pham Duc Vinh', N'090900028'),
    (N'student29@lmm.com', N'Nguyen Anh Vu', N'090900029'),
    (N'student30@lmm.com', N'Tran Phuong Yen', N'090900030'),
    (N'student31@lmm.com', N'Le Bao An', N'090900031'),
    (N'student32@lmm.com', N'Vo Minh Duc', N'090900032'),
    (N'student33@lmm.com', N'Bui Quynh Giao', N'090900033'),
    (N'student34@lmm.com', N'Dang Thanh Ha', N'090900034'),
    (N'student35@lmm.com', N'Hoang Quang Hieu', N'090900035'),
    (N'student36@lmm.com', N'Do Minh Khang', N'090900036'),
    (N'student37@lmm.com', N'Pham Thu Ly', N'090900037'),
    (N'student38@lmm.com', N'Nguyen Gia Minh', N'090900038'),
    (N'student39@lmm.com', N'Tran Nhat Nam', N'090900039'),
    (N'student40@lmm.com', N'Le Hoang Oanh', N'090900040'),
    (N'student41@lmm.com', N'Vo Minh Phu', N'090900041'),
    (N'student42@lmm.com', N'Bui Ngoc Quynh', N'090900042'),
    (N'student43@lmm.com', N'Dang Thanh Son', N'090900043'),
    (N'student44@lmm.com', N'Hoang Thien Trang', N'090900044'),
    (N'student45@lmm.com', N'Do Gia Uyen', N'090900045'),
    (N'student46@lmm.com', N'Pham Minh Vy', N'090900046'),
    (N'student47@lmm.com', N'Nguyen Hai Yen', N'090900047'),
    (N'student48@lmm.com', N'Tran Huu Zang', N'090900048'),
    (N'student49@lmm.com', N'Le Nhat Anh', N'090900049'),
    (N'student50@lmm.com', N'Vo Bao Chau', N'090900050');

INSERT INTO [AspNetUsers]
(
    [Id], [FullName], [Phone], [Avatar], [DateOfBirth], [Address], [IsActive], [CreatedAt],
    [UserName], [NormalizedUserName], [Email], [NormalizedEmail],
    [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],
    [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd],
    [LockoutEnabled], [AccessFailedCount]
)
SELECT
    NEWID(),
    s.FullName,
    s.Phone,
    NULL, NULL, NULL, 1, GETDATE(),
    s.Email, UPPER(s.Email), s.Email, UPPER(s.Email),
    1, NULL, NEWID(), NEWID(),
    s.Phone, 0, 0, NULL, 1, 0
FROM @Students s
WHERE NOT EXISTS (
    SELECT 1 FROM [AspNetUsers] u WHERE u.[NormalizedEmail] = UPPER(s.Email)
);

INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
SELECT u.[Id], @StudentRoleId
FROM [AspNetUsers] u
INNER JOIN @Students s ON UPPER(s.Email) = u.[NormalizedEmail]
WHERE NOT EXISTS (
    SELECT 1 FROM [AspNetUserRoles] ur
    WHERE ur.[UserId] = u.[Id] AND ur.[RoleId] = @StudentRoleId
);

PRINT '--- Seeded 50 students ---';

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

PRINT '--- Ensured 5 courses ---';

INSERT INTO [Rooms] ([Name], [Capacity], [Location], [IsActive])
SELECT x.[Name], x.[Capacity], x.[Location], 1
FROM (VALUES
    (N'Room C301', 30, N'Floor 3'),
    (N'Room C302', 28, N'Floor 3'),
    (N'Room D401', 35, N'Floor 4'),
    (N'Room D402', 30, N'Floor 4')
) x([Name], [Capacity], [Location])
WHERE NOT EXISTS (SELECT 1 FROM [Rooms] r WHERE r.[Name] = x.[Name]);

DECLARE @T1 NVARCHAR(450) = (SELECT [Id] FROM [AspNetUsers] WHERE [NormalizedEmail] = N'TEACHER01@LMM.COM');
DECLARE @T2 NVARCHAR(450) = (SELECT [Id] FROM [AspNetUsers] WHERE [NormalizedEmail] = N'TEACHER02@LMM.COM');
DECLARE @T3 NVARCHAR(450) = (SELECT [Id] FROM [AspNetUsers] WHERE [NormalizedEmail] = N'TEACHER03@LMM.COM');
DECLARE @T4 NVARCHAR(450) = (SELECT [Id] FROM [AspNetUsers] WHERE [NormalizedEmail] = N'TEACHER04@LMM.COM');
DECLARE @T5 NVARCHAR(450) = (SELECT [Id] FROM [AspNetUsers] WHERE [NormalizedEmail] = N'TEACHER05@LMM.COM');
DECLARE @T6 NVARCHAR(450) = (SELECT [Id] FROM [AspNetUsers] WHERE [NormalizedEmail] = N'TEACHER06@LMM.COM');
DECLARE @T7 NVARCHAR(450) = (SELECT [Id] FROM [AspNetUsers] WHERE [NormalizedEmail] = N'TEACHER07@LMM.COM');

DECLARE @R1 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room A101');
DECLARE @R2 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room B203');
DECLARE @R3 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room C301');
DECLARE @R4 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room C302');
DECLARE @R5 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room D401');
DECLARE @R6 INT = (SELECT [Id] FROM [Rooms] WHERE [Name] = N'Room D402');

DECLARE @C1 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'English Foundation A1');
DECLARE @C2 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'IELTS Intensive 5.5+');
DECLARE @C3 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'TOEIC Bridge 450+');
DECLARE @C4 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'English Communication B1');
DECLARE @C5 INT = (SELECT [Id] FROM [Courses] WHERE [Name] = N'Business English Essentials');

INSERT INTO [Classes] ([Name], [CourseId], [TeacherId], [RoomId], [StartDate], [EndDate], [MaxStudents], [Status])
SELECT x.[Name], x.[CourseId], x.[TeacherId], x.[RoomId], x.[StartDate], x.[EndDate], 25, x.[Status]
FROM (VALUES
    (N'SEED-A1-01', @C1, @T1, @R1, CAST(DATEADD(DAY, 5, GETDATE()) AS DATE),  CAST(DATEADD(DAY, 70, GETDATE()) AS DATE), 0),
    (N'SEED-A1-02', @C1, @T2, @R2, CAST(DATEADD(DAY, 10, GETDATE()) AS DATE), CAST(DATEADD(DAY, 75, GETDATE()) AS DATE), 0),
    (N'SEED-IELTS-01', @C2, @T3, @R3, CAST(DATEADD(DAY, 6, GETDATE()) AS DATE),  CAST(DATEADD(DAY, 90, GETDATE()) AS DATE), 0),
    (N'SEED-IELTS-02', @C2, @T4, @R4, CAST(DATEADD(DAY, 12, GETDATE()) AS DATE), CAST(DATEADD(DAY, 96, GETDATE()) AS DATE), 0),
    (N'SEED-TOEIC-01', @C3, @T5, @R5, CAST(DATEADD(DAY, 7, GETDATE()) AS DATE),  CAST(DATEADD(DAY, 82, GETDATE()) AS DATE), 0),
    (N'SEED-TOEIC-02', @C3, @T6, @R6, CAST(DATEADD(DAY, 13, GETDATE()) AS DATE), CAST(DATEADD(DAY, 88, GETDATE()) AS DATE), 0),
    (N'SEED-B1-01', @C4, @T7, @R3, CAST(DATEADD(DAY, 8, GETDATE()) AS DATE),  CAST(DATEADD(DAY, 78, GETDATE()) AS DATE), 0),
    (N'SEED-B1-02', @C4, @T1, @R4, CAST(DATEADD(DAY, 14, GETDATE()) AS DATE), CAST(DATEADD(DAY, 84, GETDATE()) AS DATE), 0),
    (N'SEED-BUS-01', @C5, @T2, @R5, CAST(DATEADD(DAY, 9, GETDATE()) AS DATE),  CAST(DATEADD(DAY, 86, GETDATE()) AS DATE), 0),
    (N'SEED-BUS-02', @C5, @T3, @R6, CAST(DATEADD(DAY, 15, GETDATE()) AS DATE), CAST(DATEADD(DAY, 92, GETDATE()) AS DATE), 0)
) x([Name], [CourseId], [TeacherId], [RoomId], [StartDate], [EndDate], [Status])
WHERE x.CourseId IS NOT NULL
  AND x.TeacherId IS NOT NULL
  AND x.RoomId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM [Classes] c WHERE c.[Name] = x.[Name]);

PRINT '--- Seeded classes ---';

INSERT INTO [ClassSchedules] ([ClassId], [DayOfWeek], [StartTime], [EndTime])
SELECT c.[Id], s.[DayOfWeek], s.[StartTime], s.[EndTime]
FROM [Classes] c
JOIN (VALUES
    (N'SEED-A1-01', 1, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'SEED-A1-01', 3, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'SEED-A1-02', 2, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
    (N'SEED-A1-02', 4, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
    (N'SEED-IELTS-01', 2, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME)),
    (N'SEED-IELTS-01', 4, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME)),
    (N'SEED-IELTS-02', 1, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
    (N'SEED-IELTS-02', 3, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
    (N'SEED-TOEIC-01', 1, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
    (N'SEED-TOEIC-01', 5, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
    (N'SEED-TOEIC-02', 2, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
    (N'SEED-TOEIC-02', 5, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
    (N'SEED-B1-01', 3, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME)),
    (N'SEED-B1-01', 6, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME)),
    (N'SEED-B1-02', 2, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'SEED-B1-02', 6, CAST('08:30:00' AS TIME), CAST('10:30:00' AS TIME)),
    (N'SEED-BUS-01', 4, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
    (N'SEED-BUS-01', 6, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
    (N'SEED-BUS-02', 1, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME)),
    (N'SEED-BUS-02', 5, CAST('16:30:00' AS TIME), CAST('18:30:00' AS TIME))
) s([ClassName], [DayOfWeek], [StartTime], [EndTime]) ON c.[Name] = s.[ClassName]
WHERE NOT EXISTS (
    SELECT 1
    FROM [ClassSchedules] cs
    WHERE cs.[ClassId] = c.[Id]
      AND cs.[DayOfWeek] = s.[DayOfWeek]
      AND cs.[StartTime] = s.[StartTime]
      AND cs.[EndTime] = s.[EndTime]
);

PRINT '--- Seeded class schedules ---';

DECLARE @SeedClasses TABLE (
    ClassId INT NOT NULL,
    Rn INT NOT NULL
);

INSERT INTO @SeedClasses (ClassId, Rn)
SELECT c.[Id], ROW_NUMBER() OVER (ORDER BY c.[Id])
FROM [Classes] c
WHERE c.[Name] LIKE N'SEED-%';

DECLARE @ClassCount INT = (SELECT COUNT(1) FROM @SeedClasses);

IF @ClassCount > 0
BEGIN
    ;WITH StudentList AS
    (
        SELECT
            u.[Id] AS StudentId,
            ROW_NUMBER() OVER (ORDER BY u.[Email]) AS Rn
        FROM [AspNetUsers] u
        INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id] AND ur.[RoleId] = @StudentRoleId
        WHERE u.[Email] LIKE N'student%@lmm.com'
    )
    INSERT INTO [Enrollments] ([StudentId], [ClassId], [EnrollDate], [Status])
    SELECT
        s.StudentId,
        c1.ClassId,
        GETDATE(),
        1
    FROM StudentList s
    INNER JOIN @SeedClasses c1 ON c1.Rn = ((s.Rn - 1) % @ClassCount) + 1
    WHERE NOT EXISTS (
        SELECT 1 FROM [Enrollments] e
        WHERE e.[StudentId] = s.StudentId AND e.[ClassId] = c1.ClassId
    );

    ;WITH StudentList AS
    (
        SELECT
            u.[Id] AS StudentId,
            ROW_NUMBER() OVER (ORDER BY u.[Email]) AS Rn
        FROM [AspNetUsers] u
        INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id] AND ur.[RoleId] = @StudentRoleId
        WHERE u.[Email] LIKE N'student%@lmm.com'
    )
    INSERT INTO [Enrollments] ([StudentId], [ClassId], [EnrollDate], [Status])
    SELECT
        s.StudentId,
        c2.ClassId,
        GETDATE(),
        1
    FROM StudentList s
    INNER JOIN @SeedClasses c2 ON c2.Rn = (s.Rn % @ClassCount) + 1
    WHERE NOT EXISTS (
        SELECT 1 FROM [Enrollments] e
        WHERE e.[StudentId] = s.StudentId AND e.[ClassId] = c2.ClassId
    );
END;

PRINT '--- Assigned students to classes ---';
PRINT '=== SEED BULK DATA COMPLETED ===';
GO

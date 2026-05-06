USE [LMMEducation];

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRAN;

    PRINT '=== START SEED 5 IN-PROGRESS DEMO CLASSES ===';

    IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'TEACHER')
    BEGIN
        INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
        VALUES (NEWID(), N'Teacher', N'TEACHER', NEWID());
    END;

    IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'STUDENT')
    BEGIN
        INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
        VALUES (NEWID(), N'Student', N'STUDENT', NEWID());
    END;

    DECLARE @TeacherRoleId NVARCHAR(450);
    DECLARE @StudentRoleId NVARCHAR(450);

    SELECT TOP 1
        @TeacherRoleId = [Id]
    FROM [AspNetRoles]
    WHERE [NormalizedName] = N'TEACHER';

    SELECT TOP 1
        @StudentRoleId = [Id]
    FROM [AspNetRoles]
    WHERE [NormalizedName] = N'STUDENT';

    DECLARE @TeacherPasswordHash NVARCHAR(MAX);
    DECLARE @StudentPasswordHash NVARCHAR(MAX);

    SELECT TOP 1
        @TeacherPasswordHash = u.[PasswordHash]
    FROM [AspNetUsers] u
    INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id]
    WHERE ur.[RoleId] = @TeacherRoleId
      AND u.[PasswordHash] IS NOT NULL
    ORDER BY CASE WHEN u.[NormalizedEmail] = N'TEACHER@LMM.COM' THEN 0 ELSE 1 END,
             u.[NormalizedEmail];

    SELECT TOP 1
        @StudentPasswordHash = u.[PasswordHash]
    FROM [AspNetUsers] u
    INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id]
    WHERE ur.[RoleId] = @StudentRoleId
      AND u.[PasswordHash] IS NOT NULL
    ORDER BY CASE WHEN u.[NormalizedEmail] = N'STUDENT@LMM.COM' THEN 0 ELSE 1 END,
             u.[NormalizedEmail];

    DECLARE @DemoTeachers TABLE
    (
        [Email] NVARCHAR(256) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(15) NULL
    );

    INSERT INTO @DemoTeachers ([Email], [FullName], [Phone])
    VALUES
        (N'demo.teacher01@lmm.com', N'Demo Teacher 01', N'091200001'),
        (N'demo.teacher02@lmm.com', N'Demo Teacher 02', N'091200002'),
        (N'demo.teacher03@lmm.com', N'Demo Teacher 03', N'091200003'),
        (N'demo.teacher04@lmm.com', N'Demo Teacher 04', N'091200004'),
        (N'demo.teacher05@lmm.com', N'Demo Teacher 05', N'091200005');

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
        t.[FullName],
        t.[Phone],
        NULL, NULL, NULL, 1, GETDATE(),
        t.[Email], UPPER(t.[Email]), t.[Email], UPPER(t.[Email]),
        1, @TeacherPasswordHash, NEWID(), NEWID(),
        t.[Phone], 0, 0, NULL, 1, 0
    FROM @DemoTeachers t
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [AspNetUsers] u
        WHERE u.[NormalizedEmail] = UPPER(t.[Email])
    );

    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    SELECT u.[Id], @TeacherRoleId
    FROM [AspNetUsers] u
    INNER JOIN @DemoTeachers t ON u.[NormalizedEmail] = UPPER(t.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [AspNetUserRoles] ur
        WHERE ur.[UserId] = u.[Id]
          AND ur.[RoleId] = @TeacherRoleId
    );

    PRINT '--- Ensured 5 demo teachers ---';

    DECLARE @DemoStudents TABLE
    (
        [Email] NVARCHAR(256) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(15) NULL
    );

    INSERT INTO @DemoStudents ([Email], [FullName], [Phone])
    VALUES
        (N'demo.student01@lmm.com', N'Demo Student 01', N'090800001'),
        (N'demo.student02@lmm.com', N'Demo Student 02', N'090800002'),
        (N'demo.student03@lmm.com', N'Demo Student 03', N'090800003'),
        (N'demo.student04@lmm.com', N'Demo Student 04', N'090800004'),
        (N'demo.student05@lmm.com', N'Demo Student 05', N'090800005'),
        (N'demo.student06@lmm.com', N'Demo Student 06', N'090800006'),
        (N'demo.student07@lmm.com', N'Demo Student 07', N'090800007'),
        (N'demo.student08@lmm.com', N'Demo Student 08', N'090800008'),
        (N'demo.student09@lmm.com', N'Demo Student 09', N'090800009'),
        (N'demo.student10@lmm.com', N'Demo Student 10', N'090800010'),
        (N'demo.student11@lmm.com', N'Demo Student 11', N'090800011'),
        (N'demo.student12@lmm.com', N'Demo Student 12', N'090800012'),
        (N'demo.student13@lmm.com', N'Demo Student 13', N'090800013'),
        (N'demo.student14@lmm.com', N'Demo Student 14', N'090800014'),
        (N'demo.student15@lmm.com', N'Demo Student 15', N'090800015'),
        (N'demo.student16@lmm.com', N'Demo Student 16', N'090800016'),
        (N'demo.student17@lmm.com', N'Demo Student 17', N'090800017'),
        (N'demo.student18@lmm.com', N'Demo Student 18', N'090800018'),
        (N'demo.student19@lmm.com', N'Demo Student 19', N'090800019'),
        (N'demo.student20@lmm.com', N'Demo Student 20', N'090800020'),
        (N'demo.student21@lmm.com', N'Demo Student 21', N'090800021'),
        (N'demo.student22@lmm.com', N'Demo Student 22', N'090800022'),
        (N'demo.student23@lmm.com', N'Demo Student 23', N'090800023'),
        (N'demo.student24@lmm.com', N'Demo Student 24', N'090800024'),
        (N'demo.student25@lmm.com', N'Demo Student 25', N'090800025');

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
        s.[FullName],
        s.[Phone],
        NULL, NULL, NULL, 1, GETDATE(),
        s.[Email], UPPER(s.[Email]), s.[Email], UPPER(s.[Email]),
        1, @StudentPasswordHash, NEWID(), NEWID(),
        s.[Phone], 0, 0, NULL, 1, 0
    FROM @DemoStudents s
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [AspNetUsers] u
        WHERE u.[NormalizedEmail] = UPPER(s.[Email])
    );

    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    SELECT u.[Id], @StudentRoleId
    FROM [AspNetUsers] u
    INNER JOIN @DemoStudents s ON u.[NormalizedEmail] = UPPER(s.[Email])
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [AspNetUserRoles] ur
        WHERE ur.[UserId] = u.[Id]
          AND ur.[RoleId] = @StudentRoleId
    );

    PRINT '--- Ensured 25 demo students ---';

    DECLARE @SeedCourses TABLE
    (
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Fee] DECIMAL(18,2) NOT NULL,
        [DurationInHours] INT NOT NULL
    );

    INSERT INTO @SeedCourses ([Name], [Description], [Fee], [DurationInHours])
    VALUES
        (N'English Foundation A1', N'Beginner English communication and grammar.', CAST(3500000.00 AS DECIMAL(18,2)), 48),
        (N'IELTS Intensive 5.5+', N'Focused IELTS training for band 5.5 and above.', CAST(5200000.00 AS DECIMAL(18,2)), 72),
        (N'TOEIC Bridge 450+', N'Build core TOEIC listening and reading skills.', CAST(4200000.00 AS DECIMAL(18,2)), 60),
        (N'English Communication B1', N'Develop practical communication in workplace settings.', CAST(3900000.00 AS DECIMAL(18,2)), 54),
        (N'Business English Essentials', N'Email, meeting, and presentation skills in English.', CAST(4800000.00 AS DECIMAL(18,2)), 66);

    INSERT INTO [Courses] ([Name], [Description], [Fee], [DurationInHours], [IsActive], [CreatedAt])
    SELECT sc.[Name], sc.[Description], sc.[Fee], sc.[DurationInHours], 1, GETDATE()
    FROM @SeedCourses sc
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [Courses] c
        WHERE c.[Name] = sc.[Name]
    );

    DECLARE @SeedRooms TABLE
    (
        [Name] NVARCHAR(50) NOT NULL,
        [Capacity] INT NOT NULL,
        [Location] NVARCHAR(200) NULL
    );

    INSERT INTO @SeedRooms ([Name], [Capacity], [Location])
    VALUES
        (N'Room E501', 25, N'Floor 5'),
        (N'Room E502', 25, N'Floor 5'),
        (N'Room E503', 25, N'Floor 5'),
        (N'Room E504', 25, N'Floor 5'),
        (N'Room E505', 25, N'Floor 5');

    INSERT INTO [Rooms] ([Name], [Capacity], [Location], [IsActive])
    SELECT sr.[Name], sr.[Capacity], sr.[Location], 1
    FROM @SeedRooms sr
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [Rooms] r
        WHERE r.[Name] = sr.[Name]
    );

    PRINT '--- Ensured courses and rooms ---';

    DECLARE @DemoClasses TABLE
    (
        [ClassName] NVARCHAR(100) NOT NULL,
        [CourseName] NVARCHAR(200) NOT NULL,
        [TeacherEmail] NVARCHAR(256) NOT NULL,
        [RoomName] NVARCHAR(50) NOT NULL,
        [StartDate] DATE NOT NULL,
        [EndDate] DATE NOT NULL,
        [MaxStudents] INT NOT NULL,
        [Status] INT NOT NULL
    );

    INSERT INTO @DemoClasses
    (
        [ClassName], [CourseName], [TeacherEmail], [RoomName],
        [StartDate], [EndDate], [MaxStudents], [Status]
    )
    VALUES
        (N'DEMO-LIVE-A1-01', N'English Foundation A1', N'demo.teacher01@lmm.com', N'Room E501',
         CAST(DATEADD(DAY, -12, GETDATE()) AS DATE), CAST(DATEADD(DAY, 54, GETDATE()) AS DATE), 20, 1),
        (N'DEMO-LIVE-IELTS-01', N'IELTS Intensive 5.5+', N'demo.teacher02@lmm.com', N'Room E502',
         CAST(DATEADD(DAY, -10, GETDATE()) AS DATE), CAST(DATEADD(DAY, 62, GETDATE()) AS DATE), 20, 1),
        (N'DEMO-LIVE-TOEIC-01', N'TOEIC Bridge 450+', N'demo.teacher03@lmm.com', N'Room E503',
         CAST(DATEADD(DAY, -8, GETDATE()) AS DATE), CAST(DATEADD(DAY, 52, GETDATE()) AS DATE), 20, 1),
        (N'DEMO-LIVE-B1-01', N'English Communication B1', N'demo.teacher04@lmm.com', N'Room E504',
         CAST(DATEADD(DAY, -14, GETDATE()) AS DATE), CAST(DATEADD(DAY, 46, GETDATE()) AS DATE), 20, 1),
        (N'DEMO-LIVE-BUS-01', N'Business English Essentials', N'demo.teacher05@lmm.com', N'Room E505',
         CAST(DATEADD(DAY, -6, GETDATE()) AS DATE), CAST(DATEADD(DAY, 58, GETDATE()) AS DATE), 20, 1);

    INSERT INTO [Classes] ([Name], [CourseId], [TeacherId], [RoomId], [StartDate], [EndDate], [MaxStudents], [Status])
    SELECT
        dc.[ClassName],
        c.[Id],
        u.[Id],
        r.[Id],
        dc.[StartDate],
        dc.[EndDate],
        dc.[MaxStudents],
        dc.[Status]
    FROM @DemoClasses dc
    INNER JOIN [Courses] c ON c.[Name] = dc.[CourseName]
    INNER JOIN [AspNetUsers] u ON u.[NormalizedEmail] = UPPER(dc.[TeacherEmail])
    INNER JOIN [Rooms] r ON r.[Name] = dc.[RoomName]
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [Classes] cls
        WHERE cls.[Name] = dc.[ClassName]
    );

    PRINT '--- Ensured 5 in-progress classes ---';

    DECLARE @DemoSchedules TABLE
    (
        [ClassName] NVARCHAR(100) NOT NULL,
        [DayOfWeek] INT NOT NULL,
        [StartTime] TIME NOT NULL,
        [EndTime] TIME NOT NULL
    );

    INSERT INTO @DemoSchedules ([ClassName], [DayOfWeek], [StartTime], [EndTime])
    VALUES
        (N'DEMO-LIVE-A1-01', 1, CAST('08:00:00' AS TIME), CAST('10:00:00' AS TIME)),
        (N'DEMO-LIVE-A1-01', 3, CAST('08:00:00' AS TIME), CAST('10:00:00' AS TIME)),
        (N'DEMO-LIVE-A1-01', 5, CAST('08:00:00' AS TIME), CAST('10:00:00' AS TIME)),
        (N'DEMO-LIVE-IELTS-01', 2, CAST('18:30:00' AS TIME), CAST('20:30:00' AS TIME)),
        (N'DEMO-LIVE-IELTS-01', 4, CAST('18:30:00' AS TIME), CAST('20:30:00' AS TIME)),
        (N'DEMO-LIVE-TOEIC-01', 1, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
        (N'DEMO-LIVE-TOEIC-01', 3, CAST('13:30:00' AS TIME), CAST('15:30:00' AS TIME)),
        (N'DEMO-LIVE-B1-01', 2, CAST('09:00:00' AS TIME), CAST('11:00:00' AS TIME)),
        (N'DEMO-LIVE-B1-01', 6, CAST('09:00:00' AS TIME), CAST('11:00:00' AS TIME)),
        (N'DEMO-LIVE-BUS-01', 4, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME)),
        (N'DEMO-LIVE-BUS-01', 6, CAST('19:00:00' AS TIME), CAST('21:00:00' AS TIME));

    INSERT INTO [ClassSchedules] ([ClassId], [DayOfWeek], [StartTime], [EndTime])
    SELECT
        cls.[Id],
        ds.[DayOfWeek],
        ds.[StartTime],
        ds.[EndTime]
    FROM @DemoSchedules ds
    INNER JOIN [Classes] cls ON cls.[Name] = ds.[ClassName]
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [ClassSchedules] cs
        WHERE cs.[ClassId] = cls.[Id]
          AND cs.[DayOfWeek] = ds.[DayOfWeek]
          AND cs.[StartTime] = ds.[StartTime]
          AND cs.[EndTime] = ds.[EndTime]
    );

    PRINT '--- Ensured class schedules ---';

    DECLARE @TargetClasses TABLE
    (
        [ClassId] INT NOT NULL,
        [RowNo] INT NOT NULL
    );

    INSERT INTO @TargetClasses ([ClassId], [RowNo])
    SELECT
        cls.[Id],
        ROW_NUMBER() OVER (ORDER BY cls.[Name])
    FROM [Classes] cls
    INNER JOIN @DemoClasses dc ON dc.[ClassName] = cls.[Name];

    DECLARE @ClassCount INT;

    SELECT
        @ClassCount = COUNT(1)
    FROM @TargetClasses;

    IF @ClassCount > 0
    BEGIN
        ;WITH StudentList AS
        (
            SELECT
                u.[Id] AS [StudentId],
                ROW_NUMBER() OVER (ORDER BY u.[NormalizedEmail]) AS [RowNo]
            FROM [AspNetUsers] u
            INNER JOIN @DemoStudents ds ON u.[NormalizedEmail] = UPPER(ds.[Email])
        )
        INSERT INTO [Enrollments] ([StudentId], [ClassId], [EnrollDate], [Status])
        SELECT
            sl.[StudentId],
            tc.[ClassId],
            DATEADD(DAY, -CAST(sl.[RowNo] AS INT), GETDATE()),
            1
        FROM StudentList sl
        INNER JOIN @TargetClasses tc
            ON tc.[RowNo] = ((sl.[RowNo] - 1) % @ClassCount) + 1
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM [Enrollments] e
            WHERE e.[StudentId] = sl.[StudentId]
              AND e.[ClassId] = tc.[ClassId]
        );
    END;

    PRINT '--- Ensured approved enrollments for demo students ---';
    PRINT '=== SEED 5 IN-PROGRESS DEMO CLASSES COMPLETED ===';

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK;

    THROW;
END CATCH;

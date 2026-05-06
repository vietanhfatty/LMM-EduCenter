-- ============================================================
-- LMM Education - SQL Server Database Creation Script
-- He thong quan ly giao duc trung tam day hoc truc tiep
-- Compatible with: ASP.NET Identity + EF Core 8
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'LMMEducation')
BEGIN
    ALTER DATABASE [LMMEducation] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [LMMEducation];
END
GO

CREATE DATABASE [LMMEducation];
GO

USE [LMMEducation];
GO

-- ============================================================
-- PART 1: ASP.NET Identity Tables
-- ============================================================

CREATE TABLE [AspNetRoles] (
    [Id]               NVARCHAR(450)  NOT NULL,
    [Name]             NVARCHAR(256)  NULL,
    [NormalizedName]   NVARCHAR(256)  NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [AspNetRoles]([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE TABLE [AspNetUsers] (
    [Id]                   NVARCHAR(450)  NOT NULL,
    [FullName]             NVARCHAR(100)  NOT NULL,
    [Phone]                NVARCHAR(15)   NULL,
    [Avatar]               NVARCHAR(MAX)  NULL,
    [DateOfBirth]          DATETIME2      NULL,
    [Address]              NVARCHAR(200)  NULL,
    [IsActive]             BIT            NOT NULL DEFAULT 1,
    [CreatedAt]            DATETIME2      NOT NULL DEFAULT GETDATE(),
    [UserName]             NVARCHAR(256)  NULL,
    [NormalizedUserName]   NVARCHAR(256)  NULL,
    [Email]                NVARCHAR(256)  NULL,
    [NormalizedEmail]      NVARCHAR(256)  NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR(MAX)  NULL,
    [SecurityStamp]        NVARCHAR(MAX)  NULL,
    [ConcurrencyStamp]     NVARCHAR(MAX)  NULL,
    [PhoneNumber]          NVARCHAR(MAX)  NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [AspNetUsers]([NormalizedEmail]);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [AspNetUsers]([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id]         INT            IDENTITY(1,1) NOT NULL,
    [RoleId]     NVARCHAR(450)  NOT NULL,
    [ClaimType]  NVARCHAR(MAX)  NULL,
    [ClaimValue] NVARCHAR(MAX)  NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
        FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId]
    ON [AspNetRoleClaims]([RoleId]);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id]         INT            IDENTITY(1,1) NOT NULL,
    [UserId]     NVARCHAR(450)  NOT NULL,
    [ClaimType]  NVARCHAR(MAX)  NULL,
    [ClaimValue] NVARCHAR(MAX)  NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId]
    ON [AspNetUserClaims]([UserId]);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider]       NVARCHAR(128) NOT NULL,
    [ProviderKey]         NVARCHAR(128) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId]              NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId]
    ON [AspNetUserLogins]([UserId]);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
        FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId]
    ON [AspNetUserRoles]([RoleId]);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId]        NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [Name]          NVARCHAR(128) NOT NULL,
    [Value]         NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);
GO

-- ============================================================
-- PART 2: Application Tables
-- ============================================================

-- -----------------------------------------------
-- Courses - Khoa hoc
-- -----------------------------------------------
CREATE TABLE [Courses] (
    [Id]              INT            IDENTITY(1,1) NOT NULL,
    [Name]            NVARCHAR(200)  NOT NULL,
    [Description]     NVARCHAR(2000) NULL,
    [Fee]             DECIMAL(18,2)  NOT NULL,
    [DurationInHours] INT            NOT NULL,
    [IsActive]        BIT            NOT NULL DEFAULT 1,
    [CreatedAt]       DATETIME2      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Courses] PRIMARY KEY ([Id])
);
GO

-- -----------------------------------------------
-- Subjects - Mon hoc (thuoc Khoa hoc)
-- -----------------------------------------------
CREATE TABLE [Subjects] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [Name]        NVARCHAR(200)  NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [CourseId]    INT            NOT NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Subjects_Courses_CourseId]
        FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_Subjects_CourseId]
    ON [Subjects]([CourseId]);
GO

-- -----------------------------------------------
-- Rooms - Phong hoc
-- -----------------------------------------------
CREATE TABLE [Rooms] (
    [Id]       INT           IDENTITY(1,1) NOT NULL,
    [Name]     NVARCHAR(50)  NOT NULL,
    [Capacity] INT           NOT NULL,
    [Location] NVARCHAR(200) NULL,
    [IsActive] BIT           NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Rooms] PRIMARY KEY ([Id])
);
GO

-- -----------------------------------------------
-- Classes - Lop hoc
-- -----------------------------------------------
CREATE TABLE [Classes] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [Name]        NVARCHAR(100)  NOT NULL,
    [CourseId]    INT            NOT NULL,
    [TeacherId]   NVARCHAR(450)  NOT NULL,
    [RoomId]      INT            NOT NULL,
    [StartDate]   DATETIME2      NOT NULL,
    [EndDate]     DATETIME2      NOT NULL,
    [MaxStudents] INT            NOT NULL DEFAULT 30,
    [Status]      INT            NOT NULL DEFAULT 0,  -- 0=Upcoming, 1=InProgress, 2=Completed, 3=Cancelled
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Classes_Courses_CourseId]
        FOREIGN KEY ([CourseId]) REFERENCES [Courses]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Classes_AspNetUsers_TeacherId]
        FOREIGN KEY ([TeacherId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Classes_Rooms_RoomId]
        FOREIGN KEY ([RoomId]) REFERENCES [Rooms]([Id]) ON DELETE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX [IX_Classes_CourseId]  ON [Classes]([CourseId]);
CREATE NONCLUSTERED INDEX [IX_Classes_TeacherId] ON [Classes]([TeacherId]);
CREATE NONCLUSTERED INDEX [IX_Classes_RoomId]    ON [Classes]([RoomId]);
GO

-- -----------------------------------------------
-- ClassSchedules - Lich hoc
-- -----------------------------------------------
CREATE TABLE [ClassSchedules] (
    [Id]        INT      IDENTITY(1,1) NOT NULL,
    [ClassId]   INT      NOT NULL,
    [DayOfWeek] INT      NOT NULL,  -- 0=Sunday, 1=Monday, ..., 6=Saturday
    [StartTime] TIME     NOT NULL,
    [EndTime]   TIME     NOT NULL,
    CONSTRAINT [PK_ClassSchedules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassSchedules_Classes_ClassId]
        FOREIGN KEY ([ClassId]) REFERENCES [Classes]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_ClassSchedules_ClassId]
    ON [ClassSchedules]([ClassId]);
GO

-- -----------------------------------------------
-- Enrollments - Dang ky hoc
-- -----------------------------------------------
CREATE TABLE [Enrollments] (
    [Id]         INT           IDENTITY(1,1) NOT NULL,
    [StudentId]  NVARCHAR(450) NOT NULL,
    [ClassId]    INT           NOT NULL,
    [EnrollDate] DATETIME2     NOT NULL DEFAULT GETDATE(),
    [Status]     INT           NOT NULL DEFAULT 0,  -- 0=Pending, 1=Approved, 2=Rejected, 3=Cancelled, 4=Completed
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_AspNetUsers_StudentId]
        FOREIGN KEY ([StudentId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Enrollments_Classes_ClassId]
        FOREIGN KEY ([ClassId]) REFERENCES [Classes]([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Enrollments_StudentId_ClassId]
    ON [Enrollments]([StudentId], [ClassId]);
CREATE NONCLUSTERED INDEX [IX_Enrollments_ClassId]
    ON [Enrollments]([ClassId]);
GO

-- -----------------------------------------------
-- Attendances - Diem danh
-- -----------------------------------------------
CREATE TABLE [Attendances] (
    [Id]        INT           IDENTITY(1,1) NOT NULL,
    [ClassId]   INT           NOT NULL,
    [StudentId] NVARCHAR(450) NOT NULL,
    [Date]      DATETIME2     NOT NULL,
    [Status]    INT           NOT NULL DEFAULT 0,  -- 0=Present, 1=Absent, 2=Late, 3=Excused
    [Note]      NVARCHAR(500) NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attendances_Classes_ClassId]
        FOREIGN KEY ([ClassId]) REFERENCES [Classes]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Attendances_AspNetUsers_StudentId]
        FOREIGN KEY ([StudentId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Attendances_ClassId_StudentId_Date]
    ON [Attendances]([ClassId], [StudentId], [Date]);
CREATE NONCLUSTERED INDEX [IX_Attendances_StudentId]
    ON [Attendances]([StudentId]);
GO

-- -----------------------------------------------
-- Grades - Bang diem
-- -----------------------------------------------
CREATE TABLE [Grades] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [StudentId]   NVARCHAR(450)  NOT NULL,
    [ClassId]     INT            NOT NULL,
    [Type]        INT            NOT NULL,  -- 0=Quiz, 1=Midterm, 2=Final, 3=Assignment, 4=Participation
    [Description] NVARCHAR(200)  NULL,
    [Score]       FLOAT          NOT NULL,
    [Weight]      FLOAT          NOT NULL DEFAULT 1.0,
    [CreatedAt]   DATETIME2      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Grades] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Grades_AspNetUsers_StudentId]
        FOREIGN KEY ([StudentId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Grades_Classes_ClassId]
        FOREIGN KEY ([ClassId]) REFERENCES [Classes]([Id]) ON DELETE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX [IX_Grades_StudentId] ON [Grades]([StudentId]);
CREATE NONCLUSTERED INDEX [IX_Grades_ClassId]   ON [Grades]([ClassId]);
GO

-- -----------------------------------------------
-- Payments - Hoc phi / Thanh toan
-- -----------------------------------------------
CREATE TABLE [Payments] (
    [Id]          INT            IDENTITY(1,1) NOT NULL,
    [StudentId]   NVARCHAR(450)  NOT NULL,
    [ClassId]     INT            NOT NULL,
    [Amount]      DECIMAL(18,2)  NOT NULL,
    [Method]      INT            NOT NULL DEFAULT 0,  -- 0=Cash, 1=BankTransfer, 2=Card
    [Status]      INT            NOT NULL DEFAULT 0,  -- 0=Pending, 1=Completed, 2=Failed, 3=Refunded
    [PaymentDate] DATETIME2      NOT NULL DEFAULT GETDATE(),
    [Note]        NVARCHAR(500)  NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_AspNetUsers_StudentId]
        FOREIGN KEY ([StudentId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Payments_Classes_ClassId]
        FOREIGN KEY ([ClassId]) REFERENCES [Classes]([Id]) ON DELETE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX [IX_Payments_StudentId] ON [Payments]([StudentId]);
CREATE NONCLUSTERED INDEX [IX_Payments_ClassId]   ON [Payments]([ClassId]);
GO

-- -----------------------------------------------
-- Notifications - Thong bao
-- -----------------------------------------------
CREATE TABLE [Notifications] (
    [Id]         INT            IDENTITY(1,1) NOT NULL,
    [Title]      NVARCHAR(200)  NOT NULL,
    [Content]    NVARCHAR(2000) NOT NULL,
    [SenderId]   NVARCHAR(450)  NOT NULL,
    [TargetType] INT            NOT NULL,  -- 0=All, 1=Role, 2=Class, 3=Individual
    [TargetId]   NVARCHAR(100)  NULL,
    [CreatedAt]  DATETIME2      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_SenderId]
        FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX [IX_Notifications_SenderId]
    ON [Notifications]([SenderId]);
GO

-- -----------------------------------------------
-- NotificationReads - Trang thai da doc thong bao
-- -----------------------------------------------
CREATE TABLE [NotificationReads] (
    [Id]             INT           IDENTITY(1,1) NOT NULL,
    [NotificationId] INT           NOT NULL,
    [UserId]         NVARCHAR(450) NOT NULL,
    [ReadAt]         DATETIME2     NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_NotificationReads] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_NotificationReads_Notifications_NotificationId]
        FOREIGN KEY ([NotificationId]) REFERENCES [Notifications]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NotificationReads_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_NotificationReads_NotificationId_UserId]
    ON [NotificationReads]([NotificationId], [UserId]);
CREATE NONCLUSTERED INDEX [IX_NotificationReads_UserId]
    ON [NotificationReads]([UserId]);
GO

-- -----------------------------------------------
-- TeacherReviews - Danh gia giao vien
-- -----------------------------------------------
CREATE TABLE [TeacherReviews] (
    [Id]        INT           IDENTITY(1,1) NOT NULL,
    [StudentId] NVARCHAR(450) NOT NULL,
    [TeacherId] NVARCHAR(450) NOT NULL,
    [ClassId]   INT           NOT NULL,
    [Rating]    INT           NOT NULL,
    [Comment]   NVARCHAR(1000) NULL,
    [CreatedAt] DATETIME2     NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TeacherReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherReviews_AspNetUsers_StudentId]
        FOREIGN KEY ([StudentId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherReviews_AspNetUsers_TeacherId]
        FOREIGN KEY ([TeacherId]) REFERENCES [AspNetUsers]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [CK_TeacherReview_Rating]
        CHECK ([Rating] >= 1 AND [Rating] <= 5)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_TeacherReviews_StudentId_TeacherId_ClassId]
    ON [TeacherReviews]([StudentId], [TeacherId], [ClassId]);
CREATE NONCLUSTERED INDEX [IX_TeacherReviews_TeacherId]
    ON [TeacherReviews]([TeacherId]);
GO

PRINT '=== All tables created successfully ===';
GO
